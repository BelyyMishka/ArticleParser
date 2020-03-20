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
                /*var options = new ChromeOptions();
                options.AddArgument("--window-position=-32000,-32000");*/
                driver = new ChromeDriver(service/*, options*/);
            }

            return driver;
        }
    }
}
