using TechTalk.SpecFlow;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Provides logging capabilities for test execution, focusing on diagnostic information.
    /// Separates diagnostic logging from test result reporting to maintain clear separation of concerns.
    /// </summary>
    public class TestLogger
    {
        private readonly TestReporter _testReporter;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        public TestLogger(
            TestReporter testReporter,
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            _testReporter = testReporter;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        /// <summary>
        /// Logs diagnostic information for debugging and troubleshooting.
        /// This information is intended for developers and testers to understand test behavior.
        /// </summary>
        public void LogDiagnostic(string message)
        {
            Console.WriteLine($"[Diagnostic] {message}");
        }

        /// <summary>
        /// Logs an error with diagnostic information for the current test.
        /// This is separate from test result reporting to maintain clear separation of concerns.
        /// </summary>
        public void LogError(string errorMessage, string? screenshotPath = null)
        {
            Console.WriteLine($"[Error] {errorMessage}");
            if (screenshotPath != null)
            {
                Console.WriteLine($"[Screenshot] {screenshotPath}");
            }

            _testReporter.LogTestResult(
                _featureContext.FeatureInfo.Title,
                _scenarioContext.ScenarioInfo.Title,
                "Failed",
                TimeSpan.Zero,  // Errors can happen at any point
                errorMessage,
                screenshotPath
            );
        }
    }
}
