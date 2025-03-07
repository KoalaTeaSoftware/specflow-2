using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CoreTestFramework.Support
{
    /// <summary>
    /// Manages WebDriver lifecycle and configuration.
    /// Ensures consistent browser setup and cleanup across tests.
    /// </summary>
    public class WebDriverSupport : IDisposable
    {
        private IWebDriver? _driver;
        public IWebDriver Driver
        {
            get => _driver ?? throw new InvalidOperationException("WebDriver not initialized. Call InitializeDriver first.");
            private set => _driver = value;
        }

        public void InitializeDriver()
        {
            var options = new ChromeOptions();
            //options.AddArgument("--headless=new");
            
            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Maximize();
        }

        public void CleanupDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
                _driver = null;
            }
        }

        public void Dispose()
        {
            CleanupDriver();
        }
    }
}
