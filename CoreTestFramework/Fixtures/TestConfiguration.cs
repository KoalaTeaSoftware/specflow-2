namespace CoreTestFramework.Fixtures
{
    public class TestConfiguration
    {
        public string BaseUrl { get; private set; }

        public TestConfiguration()
        {
            // This could be loaded from appsettings.json or environment variables in a more complete implementation
            BaseUrl = "https://wessexdramas.org/";
        }
    }
}
