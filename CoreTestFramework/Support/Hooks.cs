using TechTalk.SpecFlow;
using CoreTestFramework.Fixtures;
using System;
using System.IO;
using BoDi;

namespace CoreTestFramework.Support
{
    [Binding]
    public class Hooks
    {
        private readonly WebDriverSupport _webDriverSupport;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly TestReporter _reporter;
        private readonly ScreenshotManager _screenshotManager;
        private readonly DateTime _scenarioStartTime;

        public Hooks(
            WebDriverSupport webDriverSupport, 
            ScenarioContext scenarioContext, 
            FeatureContext featureContext,
            TestReporter reporter,
            ScreenshotManager screenshotManager)
        {
            _webDriverSupport = webDriverSupport;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _reporter = reporter;
            _screenshotManager = screenshotManager;
            _scenarioStartTime = DateTime.Now;
        }

        [BeforeTestRun]
        public static void BeforeTestRun(IObjectContainer container)
        {
            var reporter = new TestReporter();
            var screenshotManager = new ScreenshotManager(reporter.ScreenshotsDirectory, reporter.RunTimestamp);
            var testConfig = new TestConfiguration();
            
            container.RegisterInstanceAs(reporter);
            container.RegisterInstanceAs(screenshotManager);
            container.RegisterInstanceAs(testConfig);
        }

        [AfterTestRun]
        public static void AfterTestRun(IObjectContainer container)
        {
            var reporter = container.Resolve<TestReporter>();
            reporter.GenerateReport();
        }

        [AfterScenario(Order = 999)]
        public void AfterScenario()
        {
            try
            {
                var duration = DateTime.Now - _scenarioStartTime;
                var featureName = _featureContext.FeatureInfo.Title;
                var scenarioName = _scenarioContext.ScenarioInfo.Title;
                string? screenshotPath = null;
                string status = "Passed";
                string? errorMessage = null;

                if (_scenarioContext.TestError != null)
                {
                    status = "Failed";
                    errorMessage = _scenarioContext.TestError.Message;
                    
                    var screenshotName = $"FinalState_{featureName}_{scenarioName}";
                    screenshotName = string.Join("_", screenshotName.Split(Path.GetInvalidFileNameChars()));
                    
                    screenshotPath = _screenshotManager.CaptureScreenshot(_webDriverSupport.Driver, screenshotName);
                    if (screenshotPath != null)
                    {
                        Console.WriteLine($"Test failed! Final state screenshot saved as: {screenshotPath}");
                        Console.WriteLine($"Error: {errorMessage}");
                    }
                }

                _reporter.AddTestResult(
                    featureName,
                    scenarioName,
                    status,
                    duration,
                    errorMessage,
                    screenshotPath
                );
            }
            finally
            {
                _webDriverSupport.Dispose();
            }
        }
    }
}
