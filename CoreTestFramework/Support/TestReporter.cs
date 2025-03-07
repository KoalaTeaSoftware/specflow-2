using System.Text;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Handles diagnostic test reporting, focusing on information not covered by SpecFlow Living Documentation.
    /// Specifically captures screenshots, error details, execution metrics, and diagnostic messages to aid in debugging and quality assurance.
    /// </summary>
    public class TestReporter
    {
        private readonly string _reportDirectory;
        private readonly string _screenshotsDirectory;
        private readonly List<TestResult> _testResults;
        private readonly List<DiagnosticMessage> _diagnosticMessages;
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
            _diagnosticMessages = new List<DiagnosticMessage>();

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
        /// Records diagnostic messages during test execution.
        /// Used for tracking test progress and debugging.
        /// </summary>
        public void LogDiagnosticMessage(string message)
        {
            var diagnosticMessage = new DiagnosticMessage
            {
                Message = message,
                Timestamp = DateTime.Now
            };

            _diagnosticMessages.Add(diagnosticMessage);
        }

        /// <summary>
        /// Generates a diagnostic report focusing on test execution details, screenshots, errors, and diagnostic messages.
        /// For feature documentation and scenario descriptions, refer to the SpecFlow Living Documentation.
        /// </summary>
        public void GenerateReport()
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Support", "TestReport.html");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    "TestReport.html template not found in Support directory. " +
                    "Please ensure the template exists at: " + templatePath);
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

            // Generate diagnostic messages HTML
            var diagnosticsHtml = new StringBuilder();
            foreach (var message in _diagnosticMessages)
            {
                diagnosticsHtml.AppendLine($@"
                    <div class='diagnostic-message'>
                        <span class='timestamp'>{message.Timestamp:HH:mm:ss}</span>
                        <span class='message'>{message.Message}</span>
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
                    </div>" : "")
                .Replace("{{DiagnosticsSection}}", _diagnosticMessages.Any() ? $@"
                    <div class='diagnostics'>
                        <h2>Diagnostic Messages</h2>
                        {diagnosticsHtml}
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

        private class DiagnosticMessage
        {
            public string Message { get; set; } = "";
            public DateTime Timestamp { get; set; }
        }
    }
}
