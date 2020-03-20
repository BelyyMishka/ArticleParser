using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArticleParser
{
    static class Parser
    {
        /// <summary>
        /// Кол-во секунд
        /// </summary>
        private const int SECONDS = 15;

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
                IWebElement firstResult = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(string.Format("(//a[@class='ddmDocTitle'])[{0}]", i + 1))));
                driver.Navigate().GoToUrl(firstResult.GetAttribute("href"));
                try
                {
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS)); 
                    firstResult = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div/div[1]/div[1]/div[2]/div[1]/div[3]/div[3]/div[1]/div[1]/div[2]/p/a[2]/span"))); 
                    Writer.WriteToFile(path, firstResult.Text);
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
            IWebElement firstResult = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(@"/html/body/div[1]/div/div[1]/div[1]/div/div[3]/form/div[4]/div[2]/div/div/section[1]/div/div[4]/div/div[1]/span/span/span[2]")));
            return int.Parse(firstResult.Text);
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
        /// <param name="count">Кол-во статей</param>
        /// <returns>Кол-во страниц</returns>
        public static int GetPages(int count)
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
            string currentURL = driver.Url;
            string newUrl;
            if(currentURL.IndexOf("&offset=") == -1)
            {
                newUrl = string.Format("{0}&offset={1}", currentURL, pageNumber); 
            }
            else
            {
                Regex regex = new Regex(@"offset=[0-9]+");
                newUrl = regex.Replace(currentURL, string.Format("offset={0}", pageNumber));
            }

            driver.Navigate().GoToUrl(newUrl);
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
