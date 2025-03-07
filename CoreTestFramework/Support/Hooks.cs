using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using BoDi;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// SpecFlow hooks for test setup and teardown.
    /// Manages WebDriver lifecycle and test reporting.
    /// </summary>
    [Binding]
    public class Hooks
    {
        private readonly TestReporter _testReporter;
        private readonly ScreenshotManager _screenshotManager;
        private readonly WebDriverSupport _webDriverSupport;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly Stopwatch _scenarioTimer;

        public Hooks(
            WebDriverSupport webDriverSupport,
            TestReporter testReporter,
            ScreenshotManager screenshotManager,
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            _webDriverSupport = webDriverSupport;
            _testReporter = testReporter;
            _screenshotManager = screenshotManager;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _scenarioTimer = new Stopwatch();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            
            _webDriverSupport.Driver = new ChromeDriver(options);
            _webDriverSupport.Driver.Manage().Window.Maximize();
            
            _scenarioTimer.Start();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _scenarioTimer.Stop();
            
            try
            {
                if (_scenarioContext.TestError != null)
                {
                    var errorMessage = _scenarioContext.TestError.Message;
                    var screenshotPath = _screenshotManager.CaptureScreenshot(_webDriverSupport.Driver, "ScenarioFailure");
                    if (screenshotPath != null)
                    {
                        Console.WriteLine($"Test failed. Final state screenshot saved as: {screenshotPath}");
                        Console.WriteLine($"Error: {errorMessage}");
                    }
                    
                    _testReporter.LogTestResult(
                        _featureContext.FeatureInfo.Title,
                        _scenarioContext.ScenarioInfo.Title,
                        "Failed",
                        _scenarioTimer.Elapsed,
                        errorMessage,
                        screenshotPath
                    );
                }
                else
                {
                    _testReporter.LogTestResult(
                        _featureContext.FeatureInfo.Title,
                        _scenarioContext.ScenarioInfo.Title,
                        "Passed",
                        _scenarioTimer.Elapsed
                    );
                }
            }
            finally
            {
                _webDriverSupport.Dispose();
            }
        }

        [BeforeTestRun]
        public static void BeforeTestRun(IObjectContainer container)
        {
            // Register core services in the correct order
            var webDriverSupport = new WebDriverSupport(container);
            container.RegisterInstanceAs(webDriverSupport);

            var testReporter = new TestReporter();
            container.RegisterInstanceAs(testReporter);

            var screenshotManager = new ScreenshotManager(
                testReporter.ScreenshotsDirectory,
                testReporter.RunTimestamp
            );
            container.RegisterInstanceAs(screenshotManager);
        }

        [AfterTestRun]
        public static void AfterTestRun(IObjectContainer container)
        {
            var reporter = container.Resolve<TestReporter>();
            reporter.GenerateReport();
        }
    }
}
