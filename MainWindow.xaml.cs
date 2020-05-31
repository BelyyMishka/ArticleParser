using System;
using System.Windows;
using System.Windows.Controls;

namespace ArticleParser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Максимальное кол-во статей
        /// </summary>
        private const int MAX_COUNT = 2000;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик кнопки Старт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartParsing(object sender, RoutedEventArgs e)
        {
            string URL = URLTextBox.Text;

            if (string.IsNullOrWhiteSpace(URL))
            {
                MessageBox.Show("Вы не вставили ссылку!");
                return;
            }

            var driver = Driver.GetInstance();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            DisableStartButton();
            DisableURLTextBox();
            EnableStopButton();

            try
            {
                driver.Navigate().GoToUrl(URL);
            }
            catch
            {
                return;
            }

            var tuple = await Parser.GetCurrentPositionAndAllAsync(driver);
            int currentPosition = tuple.currentPosition;
            int all = tuple.all;
            int count = Math.Min(all - currentPosition + 1, MAX_COUNT);
            All.Content = count.ToString();

            for (int i = 0; i < count; i++)
            {
                await Parser.ParseArticleAsync(driver, path, currentPosition, i + 1, CurrentPosition, SequenceEmptyEmailsLabel, EmptyEmailsLabel);

                if (i == count - 1)
                {
                    break;
                }
                await Parser.GoToNextArticleAsync(driver);
                currentPosition++;
            }

            Driver.GetInstance().Close();
            Driver.GetInstance().Quit();
            MessageBox.Show("Парсинг завершен!");
            Environment.Exit(0);
        }

        /// <summary>
        /// Обработчик кнопки Стоп
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopParsing(object sender, RoutedEventArgs e)
        {
            Driver.GetInstance().Close();
            Driver.GetInstance().Quit();
            Environment.Exit(0);
        }

        /// <summary>
        /// Отключить кнопку Старт
        /// </summary>
        private void DisableStartButton()
        {
            Button button = StartButton;
            if(button.IsEnabled)
            {
                button.IsEnabled = false;
            }
        }

        /// <summary>
        /// Включить кнопку Стоп
        /// </summary>
        private void EnableStopButton()
        {
            Button button = StopButton;
            if(!button.IsEnabled)
            {
                button.IsEnabled = true;
            }
        }

        /// <summary>
        /// Отключить TextBox
        /// </summary>
        private void DisableURLTextBox()
        {
            TextBox textBox = URLTextBox;
            if(textBox.IsEnabled)
            {
                textBox.IsEnabled = false;
            }
        }
    }
}
