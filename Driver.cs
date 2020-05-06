using OpenQA.Selenium.Chrome;

namespace ArticleParser
{
    static class Driver
    {
        /// <summary>
        /// Экземпляр класса ChromeDriver
        /// </summary>
        private static ChromeDriver driver;

        /// <summary>
        /// Метод для получения экземпляра ChromeDriver
        /// </summary>
        /// <returns>Экземпляр ChromeDriver</returns>
        public static ChromeDriver GetInstance()
        {
            if(driver == null)
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                driver = new ChromeDriver(service);
            }

            return driver;
        }
    }
}
