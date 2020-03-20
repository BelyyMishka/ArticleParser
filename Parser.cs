using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArticleParser
{
    static class Parser
    {
        /// <summary>
        /// Кол-во секунд
        /// </summary>
        private const int SECONDS = 10;

        /// <summary>
        /// Словарь статьи => страницы
        /// </summary>
        private static Dictionary<int, int> pages = new Dictionary<int, int>
        {
            [20] = 100,
            [50] = 40,
            [100] = 20,
            [200] = 10
        };

        /// <summary>
        /// Метод для парсинга статей со страницы
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="count">Число статей</param>
        private static void ParsePerPage(ChromeDriver driver, string path, int count)
        {
            for (var i = 0; i < count; i++)
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS));
                IWebElement firstResult = wait.Until(e => e.FindElement(By.XPath(string.Format("(//a[@class='ddmDocTitle'])[{0}]", i + 1))));
                driver.Navigate().GoToUrl(firstResult.GetAttribute("href"));
                try
                {
                    IWebElement email = driver.FindElement(By.XPath("/html/body/div/div[1]/div[1]/div[2]/div[1]/div[3]/div[3]/div[1]/div[1]/div[2]/p/a[2]/span"));
                    Writer.WriteToFile(path, email.Text);
                }
                catch
                {

                }
                
                driver.Navigate().Back();
            }
        }

        /// <summary>
        /// Асинхронный метод для парсинга статей со страницы
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="count">Число статей</param>
        /// <returns>Таск</returns>
        public async static Task ParsePerPageAsync(ChromeDriver driver, string path, int count)
        {
            await Task.Run(() => ParsePerPage(driver, path, count));
        }

        /// <summary>
        /// Метод подсчета статей на странице
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="URL">Ссылка</param>
        /// <returns>Число статей</returns>
        private static int GetArticlesPerPage(ChromeDriver driver, string URL)
        {
            try
            {
                driver.Navigate().GoToUrl(URL);
            }
            catch
            {
                driver.Close();
                driver.Quit();
                Environment.Exit(0);
            }
            
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS));
            IReadOnlyCollection<IWebElement> results = wait.Until(e => e.FindElements(By.XPath(@"//a[@class='ddmDocTitle']")));
            return results.Count;
        }

        /// <summary>
        /// Асинхронный метод подсчета статей на странице
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="URL">Ссылка</param>
        /// <returns>Таск</returns>
        public async static Task<int> GetArticlesPerPageAsync(ChromeDriver driver, string URL)
        {
            return await Task.Run(() => GetArticlesPerPage(driver, URL));
        }

        /// <summary>
        /// Метод подсчета кол-ва страниц
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="count">Кол-во статей</param>
        /// <returns>Кол-во страниц</returns>
        public static int GetPages(ChromeDriver driver, int count)
        {
            return pages[count];
        }

        /// <summary>
        /// Метод для перехода на следующую страницу
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="pageNumber">Номер страницы</param>
        private static void GoToNextPage(ChromeDriver driver, int pageNumber)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS));
            IWebElement firstResult = wait.Until(e => e.FindElement(By.CssSelector(string.Format("a[data-value='{0}']", pageNumber))));
            firstResult.Click();
        }

        /// <summary>
        /// Асинхронный метод для перехода на следующую страницу
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <returns>Таск</returns>
        public async static Task GoToNextPageAsync(ChromeDriver driver, int pageNumber)
        {
            await Task.Run(() => GoToNextPage(driver, pageNumber));
        }
    }
}
