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
        /// Records diagnostic information for a test execution.
        /// </summary>
        public void AddTestResult(
            string featureTitle,
            string scenarioTitle,
            string status,
            TimeSpan duration,
            string? errorMessage = null,
            string? screenshotPath = null)
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
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='en'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("    <title>Test Diagnostics Report</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        :root { --primary-color: #0066cc; --success-color: #28a745; --danger-color: #dc3545; }");
            html.AppendLine("        body { font-family: system-ui, -apple-system, sans-serif; margin: 0; padding: 20px; background: #f8f9fa; }");
            html.AppendLine("        .container { max-width: 1200px; margin: 0 auto; background: white; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        .header { background: var(--primary-color); color: white; padding: 20px; border-radius: 8px 8px 0 0; }");
            html.AppendLine("        .content { padding: 20px; }");
            html.AppendLine("        .metrics { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }");
            html.AppendLine("        .metric-card { background: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center; }");
            html.AppendLine("        .metric-card h3 { margin: 0 0 10px 0; color: var(--primary-color); }");
            html.AppendLine("        .metric-value { font-size: 24px; font-weight: bold; }");
            html.AppendLine("        .failures { margin-top: 30px; }");
            html.AppendLine("        .failure { background: #fff5f5; border-radius: 8px; padding: 20px; margin-bottom: 20px; }");
            html.AppendLine("        .failure h3 { color: var(--danger-color); margin: 0 0 10px 0; }");
            html.AppendLine("        .error-details { background: #f8f9fa; padding: 15px; border-radius: 4px; margin: 10px 0; }");
            html.AppendLine("        .screenshot { margin: 20px 0; }");
            html.AppendLine("        .screenshot img { max-width: 100%; border-radius: 4px; border: 1px solid #ddd; }");
            html.AppendLine("        .execution-time { color: #666; font-size: 0.9em; }");
            html.AppendLine("        .info-box { background: #e7f5ff; border-radius: 8px; padding: 15px; margin: 20px 0; }");
            html.AppendLine("        .info-box a { color: var(--primary-color); }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div class='container'>");

            // Header
            html.AppendLine("<div class='header'>");
            html.AppendLine("    <h1>Test Diagnostics Report</h1>");
            html.AppendLine($"    <p>Run Date: {_runTimestamp}</p>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='content'>");

            // Info Box
            html.AppendLine("<div class='info-box'>");
            html.AppendLine("    <p><strong>Note:</strong> This report focuses on diagnostic information and test execution details. ");
            html.AppendLine("    For feature documentation and scenario descriptions, please refer to the ");
            html.AppendLine("    <a href='LivingDoc.html'>SpecFlow Living Documentation</a>.</p>");
            html.AppendLine("</div>");

            // Execution Metrics
            var totalTests = _testResults.Count;
            var failedTests = _testResults.Count(r => r.Status == "Failed");
            var totalDuration = TimeSpan.FromSeconds(_testResults.Sum(r => r.Duration.TotalSeconds));

            html.AppendLine("<div class='metrics'>");
            html.AppendLine("    <div class='metric-card'>");
            html.AppendLine("        <h3>Total Tests</h3>");
            html.AppendLine($"        <div class='metric-value'>{totalTests}</div>");
            html.AppendLine("    </div>");
            html.AppendLine("    <div class='metric-card'>");
            html.AppendLine("        <h3>Failed Tests</h3>");
            html.AppendLine($"        <div class='metric-value' style='color: var(--danger-color)'>{failedTests}</div>");
            html.AppendLine("    </div>");
            html.AppendLine("    <div class='metric-card'>");
            html.AppendLine("        <h3>Total Duration</h3>");
            html.AppendLine($"        <div class='metric-value'>{totalDuration.TotalSeconds:F1}s</div>");
            html.AppendLine("    </div>");
            html.AppendLine("</div>");

            // Failed Tests Section
            if (failedTests > 0)
            {
                html.AppendLine("<div class='failures'>");
                html.AppendLine("    <h2>Failed Tests</h2>");

                foreach (var failure in _testResults.Where(r => r.Status == "Failed"))
                {
                    html.AppendLine("    <div class='failure'>");
                    html.AppendLine($"        <h3>{failure.ScenarioTitle}</h3>");
                    html.AppendLine($"        <div class='execution-time'>Executed at {failure.Timestamp:HH:mm:ss} (Duration: {failure.Duration.TotalSeconds:F1}s)</div>");
                    
                    if (!string.IsNullOrEmpty(failure.ErrorMessage))
                    {
                        html.AppendLine("        <div class='error-details'>");
                        html.AppendLine($"            <pre>{failure.ErrorMessage}</pre>");
                        html.AppendLine("        </div>");
                    }

                    if (!string.IsNullOrEmpty(failure.ScreenshotPath))
                    {
                        html.AppendLine("        <div class='screenshot'>");
                        html.AppendLine("            <p><strong>Failure Screenshot:</strong></p>");
                        html.AppendLine($"            <img src='Screenshots/{failure.ScreenshotPath}' alt='Test Failure Screenshot'>");
                        html.AppendLine("        </div>");
                    }

                    html.AppendLine("    </div>");
                }

                html.AppendLine("</div>");
            }

            html.AppendLine("</div>");
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            var reportPath = Path.Combine(_reportDirectory, "TestDiagnostics.html");
            File.WriteAllText(reportPath, html.ToString());
            Console.WriteLine($"\nDiagnostic report generated at: {reportPath}");
            Console.WriteLine("For feature documentation, see: LivingDoc.html in the same directory");
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
