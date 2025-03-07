using System.Text;
using TechTalk.SpecFlow;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Handles diagnostic test reporting, focusing on information not covered by SpecFlow Living Documentation.
    /// Specifically captures screenshots, error details, and execution metrics to aid in debugging and quality assurance.
    /// </summary>
    public class TestReporter
    {
        private readonly string _reportDirectory;
        private readonly string _screenshotsDirectory;
        private readonly List<TestResult> _testResults;
        private readonly string _runTimestamp;

        public TestReporter()
        {
            // Use timestamp from environment variable if available, otherwise generate new one
            _runTimestamp = Environment.GetEnvironmentVariable("TEST_RUN_TIMESTAMP") 
                ?? DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            var projectDir = AppDomain.CurrentDomain.BaseDirectory;
            while (!string.IsNullOrEmpty(projectDir) && !Directory.Exists(Path.Combine(projectDir, "CoreTestFramework")))
            {
                projectDir = Path.GetDirectoryName(projectDir);
            }
            var solutionRoot = projectDir ?? throw new InvalidOperationException("Could not find solution root directory");

            _reportDirectory = Path.Combine(solutionRoot, "TestReports", $"Run_{_runTimestamp}");
            _screenshotsDirectory = Path.Combine(_reportDirectory, "Screenshots");
            _testResults = new List<TestResult>();

            Directory.CreateDirectory(_reportDirectory);
            Directory.CreateDirectory(_screenshotsDirectory);
        }

        public string ReportDirectory => _reportDirectory;
        public string ScreenshotsDirectory => _screenshotsDirectory;
        public string RunTimestamp => _runTimestamp;

        /// <summary>
        /// Records test execution result with timing information.
        /// Used by test hooks to record final test status and duration.
        /// </summary>
        public void LogTestResult(string featureTitle, string scenarioTitle, string status, TimeSpan duration, string? errorMessage = null, string? screenshotPath = null)
        {
            var result = new TestResult
            {
                FeatureTitle = featureTitle,
                ScenarioTitle = scenarioTitle,
                Status = status,
                Duration = duration,
                ErrorMessage = errorMessage,
                ScreenshotPath = screenshotPath != null ? Path.GetFileName(screenshotPath) : null,
                Timestamp = DateTime.Now
            };

            _testResults.Add(result);
        }

        /// <summary>
        /// Generates a diagnostic report focusing on test execution details, screenshots, and errors.
        /// For feature documentation and scenario descriptions, refer to the SpecFlow Living Documentation.
        /// </summary>
        public void GenerateReport()
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Support", "TestReport.html");
            var projectTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Support", "TestReport.html");

            if (!File.Exists(templatePath) && File.Exists(projectTemplatePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(templatePath)!);
                File.Copy(projectTemplatePath, templatePath);
            }

            if (!File.Exists(templatePath))
            {
                // Create a basic template if none exists
                var basicTemplate = @"<!DOCTYPE html>
<html>
<head>
    <title>Test Execution Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .failure { border: 1px solid #ddd; padding: 10px; margin: 10px 0; }
        .error-details { background: #f8f8f8; padding: 10px; }
        .screenshot img { max-width: 800px; }
    </style>
</head>
<body>
    <h1>Test Execution Report - {{RunDate}}</h1>
    <div class='summary'>
        <p>Total Tests: {{TotalTests}}</p>
        <p>Failed Tests: {{FailedTests}}</p>
        <p>Total Duration: {{TotalDuration}}s</p>
    </div>
    {{FailuresSection}}
</body>
</html>";
                Directory.CreateDirectory(Path.GetDirectoryName(templatePath)!);
                File.WriteAllText(templatePath, basicTemplate);
            }

            var template = File.ReadAllText(templatePath);
            
            // Calculate metrics
            var totalTests = _testResults.Count;
            var failedTests = _testResults.Count(r => r.Status == "Failed");
            var totalDuration = TimeSpan.FromSeconds(_testResults.Sum(r => r.Duration.TotalSeconds));

            // Generate failures HTML
            var failuresHtml = new StringBuilder();
            foreach (var failure in _testResults.Where(r => r.Status == "Failed"))
            {
                failuresHtml.AppendLine($@"
                    <div class='failure'>
                        <h3>{failure.ScenarioTitle}</h3>
                        <div class='execution-time'>Executed at {failure.Timestamp:HH:mm:ss} (Duration: {failure.Duration.TotalSeconds:F1}s)</div>
                        {(failure.ErrorMessage != null ? $@"
                        <div class='error-details'>
                            <pre>{failure.ErrorMessage}</pre>
                        </div>" : "")}
                        {(failure.ScreenshotPath != null ? $@"
                        <div class='screenshot'>
                            <p><strong>Failure Screenshot:</strong></p>
                            <img src='Screenshots/{failure.ScreenshotPath}' alt='Test Failure Screenshot'>
                        </div>" : "")}
                    </div>");
            }

            // Replace placeholders in template
            var report = template
                .Replace("{{RunDate}}", _runTimestamp)
                .Replace("{{TotalTests}}", totalTests.ToString())
                .Replace("{{FailedTests}}", failedTests.ToString())
                .Replace("{{TotalDuration}}", totalDuration.TotalSeconds.ToString("F1"))
                .Replace("{{FailuresSection}}", failedTests > 0 ? $@"
                    <div class='failures'>
                        <h2>Failed Tests</h2>
                        {failuresHtml}
                    </div>" : "");

            var reportPath = Path.Combine(_reportDirectory, "TestDiagnostics.html");
            File.WriteAllText(reportPath, report);
        }

        private class TestResult
        {
            public string FeatureTitle { get; set; } = "";
            public string ScenarioTitle { get; set; } = "";
            public string Status { get; set; } = "";
            public TimeSpan Duration { get; set; }
            public string? ErrorMessage { get; set; }
            public string? ScreenshotPath { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
