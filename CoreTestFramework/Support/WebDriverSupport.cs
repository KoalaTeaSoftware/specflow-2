using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using BoDi;

namespace CoreTestFramework.Support
{
    public class WebDriverSupport : IDisposable
    {
        private IWebDriver? _driver;
        private readonly IObjectContainer _container;

        public WebDriverSupport(IObjectContainer container)
        {
            _container = container;
            InitializeDriver();
        }

        private void InitializeDriver()
        {
            _driver = new ChromeDriver();
            _container.RegisterInstanceAs<IWebDriver>(_driver);
        }

        public IWebDriver Driver => _driver ?? throw new InvalidOperationException("WebDriver not initialized");

        public string GetPageTitle() => Driver.Title;

        public void NavigateToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public void Dispose()
        {
            if (_driver != null)
            {
                try
                {
                    _driver.Quit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing WebDriver: {ex.Message}");
                }
                _driver.Dispose();
                _driver = null;
            }
        }
    }
}
