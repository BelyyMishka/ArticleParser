using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArticleParser
{
    static class Parser
    {
        private const string FIRST_PAGE = "&offset=1&";

        private static void ParsePerPage(ChromeDriver driver, string path, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var link = driver.FindElement(By.XPath(string.Format("(//a[@class='ddmDocTitle'])[{0}]", i + 1)));
                string href = link.GetAttribute("href");
                driver.Navigate().GoToUrl(href);
                try
                {
                    var email = driver.FindElement(By.XPath("/html/body/div/div[1]/div[1]/div[2]/div[1]/div[3]/div[3]/div[1]/div[1]/div[2]/p/a[2]/span")).Text;
                    Writer.WriteToFile(path, email);
                }
                catch
                {

                }
                
                driver.Navigate().Back();
            }
        }

        public async static Task ParsePerPageAsync(ChromeDriver driver, string path, int count)
        {
            await Task.Run(() => ParsePerPage(driver, path, count));
        }

        private static int GetArticlesPerPage(ChromeDriver driver, string URL)
        {
            driver.Navigate().GoToUrl(URL);
            return driver.FindElements(By.XPath(@"//a[@class='ddmDocTitle']")).Count;
        }

        public async static Task<int> GetArticlesPerPageAsync(ChromeDriver driver, string URL)
        {
            return await Task.Run(() => GetArticlesPerPage(driver, URL));
        }

        public async static Task<int> GetPagesAsync(ChromeDriver driver)
        {
            return await Task.Run(() => GetPages(driver)); 
        }

        private static int GetPages(ChromeDriver driver)
        {
            return int.Parse(driver.FindElement(By.XPath(@"/html/body/div[1]/div/div[1]/div[1]/div/div[3]/form/div[4]/div[2]/div/div/section[1]/div/div[4]/div/div[2]/ul/li[7]/a")).Text);
        }

        private static void GoToNextPage(ChromeDriver driver, int pageNumber)
        {
            driver.FindElement(By.CssSelector(string.Format("a[data-value='{0}']", pageNumber))).Click();
        }

        public async static Task GoToNextPageAsync(ChromeDriver driver, int pageNumber)
        {
            await Task.Run(() => GoToNextPage(driver, pageNumber));
        }

        public static bool IsFirstPage(string URL)
        {
            return URL.IndexOf(FIRST_PAGE) == -1 ? false : true;
        }
    }
}
