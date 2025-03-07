using TechTalk.SpecFlow;
using OpenQA.Selenium;
using BoDi;
using System.Diagnostics;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Provides SpecFlow hooks for test lifecycle management.
    /// Centralizes test lifecycle events and ensures proper cleanup of resources.
    /// </summary>
    [Binding]
    public class Hooks
    {
        private readonly WebDriverSupport _webDriverSupport;
        private readonly TestReporter _testReporter;
        private readonly TestLogger _logger;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly Stopwatch _scenarioTimer;

        public Hooks(
            WebDriverSupport webDriverSupport,
            TestReporter testReporter,
            TestLogger logger,
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            _webDriverSupport = webDriverSupport;
            _testReporter = testReporter;
            _logger = logger;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _scenarioTimer = new Stopwatch();
        }

        [BeforeScenario]
        public void BeforeScenario(IObjectContainer container)
        {
            _scenarioTimer.Start();
            _webDriverSupport.InitializeDriver();
            container.RegisterInstanceAs(_webDriverSupport.Driver);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _scenarioTimer.Stop();

            // If there was an error, capture screenshot
            string? screenshotPath = null;
            if (_scenarioContext.TestError != null)
            {
                var screenshotManager = new ScreenshotManager(_testReporter.ScreenshotsDirectory);
                screenshotPath = screenshotManager.CaptureScreenshot(_webDriverSupport.Driver, "TestFailure");
            }

            // Log final test result
            _testReporter.LogTestResult(
                _featureContext.FeatureInfo.Title,
                _scenarioContext.ScenarioInfo.Title,
                _scenarioContext.TestError == null ? "Passed" : "Failed",
                _scenarioTimer.Elapsed,
                _scenarioContext.TestError?.Message,
                screenshotPath
            );

            _webDriverSupport.CleanupDriver();
        }

        [BeforeTestRun]
        public static void BeforeTestRun(IObjectContainer container)
        {
            var webDriverSupport = new WebDriverSupport();
            container.RegisterInstanceAs<WebDriverSupport>(webDriverSupport);

            var testReporter = new TestReporter();
            container.RegisterInstanceAs<TestReporter>(testReporter);

            var screenshotManager = new ScreenshotManager(testReporter.ScreenshotsDirectory);
            container.RegisterInstanceAs<ScreenshotManager>(screenshotManager);

            var testLogger = new TestLogger(testReporter);
            container.RegisterInstanceAs<TestLogger>(testLogger);
        }

        [AfterTestRun]
        public static void AfterTestRun(IObjectContainer container)
        {
            var reporter = container.Resolve<TestReporter>();
            reporter.GenerateReport();
        }
    }
}
