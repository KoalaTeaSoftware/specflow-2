using OpenQA.Selenium;
using System;
using System.IO;

namespace CoreTestFramework.Support
{
    public class ScreenshotManager
    {
        private readonly string _screenshotsDirectory;
        private readonly string _runTimestamp;

        public ScreenshotManager(string screenshotsDirectory, string runTimestamp)
        {
            _screenshotsDirectory = screenshotsDirectory ?? throw new ArgumentNullException(nameof(screenshotsDirectory));
            _runTimestamp = runTimestamp ?? throw new ArgumentNullException(nameof(runTimestamp));
        }

        public string ScreenshotsDirectory => _screenshotsDirectory;
        public string RunTimestamp => _runTimestamp;

        public string CaptureScreenshot(IWebDriver driver, string namePrefix)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var filename = $"{namePrefix}_{_runTimestamp}.png";
                var screenshotPath = Path.Combine(_screenshotsDirectory, filename);
                screenshot.SaveAsFile(screenshotPath);
                Console.WriteLine($"Screenshot saved to: {screenshotPath}");
                return filename; // Return just filename for report linking
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
