using TechTalk.SpecFlow;
using System;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Provides logging capabilities for test execution.
    /// Focuses on diagnostic information, separating logging from test result reporting.
    /// </summary>
    public class TestLogger
    {
        private readonly TestReporter _testReporter;

        public TestLogger(TestReporter testReporter)
        {
            _testReporter = testReporter;
        }

        /// <summary>
        /// Logs diagnostic information during test execution.
        /// Used for tracking test progress and debugging.
        /// </summary>
        public void LogDiagnostic(string message)
        {
            _testReporter.LogDiagnosticMessage(message);
        }

        /// <summary>
        /// Logs errors with diagnostic information for the current test.
        /// Ensures clarity in error reporting and separation from test result logging.
        /// </summary>
        public void LogError(string errorMessage, string featureTitle, string scenarioTitle, string? screenshotPath = null)
        {
            _testReporter.LogTestResult(featureTitle, scenarioTitle, "Failed", TimeSpan.Zero, errorMessage, screenshotPath);
        }
    }
}
