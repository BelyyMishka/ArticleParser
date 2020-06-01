using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ArticleParser
{
    static class Parser
    {
        /// <summary>
        /// Кол-во секунд
        /// </summary>
        private const int SECONDS = 1;

        /// <summary>
        /// Общее число пустых адресов
        /// </summary>
        private static int emptyEmails = 0;

        /// <summary>
        /// Был ли считан адрес в предыдущей статье
        /// </summary>
        private static bool isLastEmpty = false;

        /// <summary>
        /// Число пустых адресов подряд
        /// </summary>
        private static int sequenceEmptyEmails = 0;

        /// <summary>
        /// Счетсчик для проверки нужно ли обновлять sequenceEmptyEmails
        /// </summary>
        private static bool isNeedToAdd = true;

        /// <summary>
        /// Асинхронный метод получения текущей позиции и общего количества
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <returns>Текущая позиция и общее количество</returns>
        public async static Task<(int currentPosition, int all)> GetCurrentPositionAndAllAsync(ChromeDriver driver)
        {
            return await Task.Run(() => GetCurrentPositionAndAll(driver));
        }

        /// <summary>
        /// Метод получения текущей позиции и общего количества
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <returns>Текущая позиция и общее количество</returns>
        private static (int currentPosition, int all) GetCurrentPositionAndAll(ChromeDriver driver)
        {
            var tuple = (currentPosition: 1, all: 1);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS));
            IWebElement firstResult = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(@".recordPageCount")));
            string text = firstResult.Text;
            text = text.Replace(" ", "");
            Regex regex = new Regex(@"\d+", RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(text);
            tuple.currentPosition = int.Parse(matches[0].Value);
            tuple.all = int.Parse(matches[1].Value);
            return tuple;
        }

        /// <summary>
        /// Асинхронный метод парсинга адреса статьи
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="number">Номер статьи</param>
        /// <param name="currentPosition">Текущая позиция</param>
        /// <param name="currentPositionLabel">Текущая позиция на интерфейсе</param>
        /// <param name="emptyEmailsLabel">Пустые адреса на интерфейсе</param>
        /// <param name="seqEmptyEmailsLabel">Пустые адреса подряд на интерфейсе</param>
        /// <returns></returns>
        public static async Task ParseArticleAsync(ChromeDriver driver, string path, int number, int currentPosition, Label currentPositionLabel, Label seqEmptyEmailsLabel, Label emptyEmailsLabel)
        {
            await Task.Run(() => ParseArticle(driver, path, number, currentPosition, currentPositionLabel, seqEmptyEmailsLabel, emptyEmailsLabel));
        }

        /// <summary>
        /// Метод парсинга адреса статьи
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="number">Номер статьи</param>
        /// <param name="currentPosition">Текущая позиция</param>
        /// <param name="currentPositionLabel">Текущая позиция на интерфейсе</param>
        /// <param name="emptyEmailsLabel">Пустые адреса на интерфейсе</param>
        /// <param name="seqEmptyEmailsLabel">Пустые адреса подряд на интерфейсе</param>
        /// <returns></returns>
        private static void ParseArticle(ChromeDriver driver, string path, int number, int currentPosition, Label currentPositionLabel, Label seqEmptyEmailsLabel, Label emptyEmailsLabel)
        {
            try
            {
                Application.Current.Dispatcher.InvokeAsync(() => { currentPositionLabel.Content = currentPosition.ToString(); });
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS));
                var firstResult = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".corrAuthSect .anchorText"))); 
                Writer.WriteToFile(path, firstResult.Text, number);
                isLastEmpty = false;
                isNeedToAdd = true;
            }
            catch
            {
                if (isLastEmpty && isNeedToAdd)
                {
                    sequenceEmptyEmails++;
                    Application.Current.Dispatcher.InvokeAsync(() => { seqEmptyEmailsLabel.Content = sequenceEmptyEmails.ToString(); });
                    isNeedToAdd = false;
                }
                Writer.WriteToFile(path, "Не удалось получить e-mail", number);
                emptyEmails++;
                Application.Current.Dispatcher.InvokeAsync(() => { emptyEmailsLabel.Content = emptyEmails.ToString(); });
                isLastEmpty = true;
            }
        }

        /// <summary>
        /// Асинхронный метод перехода к следующей статье
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <returns></returns>
        public static async Task GoToNextArticleAsync(ChromeDriver driver)
        {
            await Task.Run(() => GoToNextArticle(driver));
        }

        /// <summary>
        /// Метод перехода к следующей статье
        /// </summary>
        /// <param name="driver">Драйвер</param>
        /// <returns></returns>
        private static void GoToNextArticle(ChromeDriver driver)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(SECONDS));
                IWebElement firstResult = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(@".nextLink a")));
                driver.Navigate().GoToUrl(firstResult.GetAttribute("href"));
            }
            catch
            {

            }
        }
    }
}