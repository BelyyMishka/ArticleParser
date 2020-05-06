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
        /// Экземпляр класса MainWindow
        /// </summary>
        public static MainWindow mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
        }

        /// <summary>
        /// Обработчик кнопки Старт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartParsing(object sender, RoutedEventArgs e)
        {
            string URL = URLTextBox.Text;

            var driver = Driver.GetInstance();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            DisableTestButton();
            DisableSlider();
            DisableStartButton();
            EnableStopButton();

            int perPageCount = await Parser.GetArticlesPerPageAsync(driver, URL);
            int totalCount = await Parser.GetArticlesCountAsync(driver, URL);
            int pages = (int)Math.Floor((decimal)totalCount / perPageCount);

            for (int i = 0; i < pages; i++ )
            {
                await Parser.ParsePerPageAsync(driver, path, perPageCount);
                if(i == pages - 1)
                {
                    break;
                }
                await Parser.GoToNextPageAsync(driver, (i + 1) * perPageCount + 1);
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
        /// Отключить слайдер
        /// </summary>
        private void DisableSlider()
        {
            Slider slider = Slider;
            if(slider.IsEnabled)
            {
                slider.IsEnabled = false;
            }
        }

        /// <summary>
        /// Поставить макс. значение слайдеру
        /// </summary>
        /// <param name="max"></param>
        private void SetMaximumSlider(int max)
        {
            Slider slider = Slider;
            slider.Maximum = max;
        }

        /// <summary>
        /// Включить кнопку Тест
        /// </summary>
        private void EnableTestButton()
        {
            Button button = TestButton;
            if (!button.IsEnabled)
            {
                button.IsEnabled = true;
            }
        }

        /// <summary>
        /// Включить Слайдер
        /// </summary>
        private void EnableSlider()
        {
            Slider slider = Slider;
            if (!slider.IsEnabled)
            {
                slider.IsEnabled = true;
            }
        }

        /// <summary>
        /// Событие ввода текста в поле ввода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void URLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableTestButton();
            DisableStartButton();
            DisableSlider();
        }

        /// <summary>
        /// Отключить кнопку Тест
        /// </summary>
        private void DisableTestButton()
        {
            Button button = TestButton;
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
        /// Включить кнопку Старт
        /// </summary>
        private void EnableStartButton()
        {
            Button button = StartButton;
            if (!button.IsEnabled)
            {
                button.IsEnabled = true;
            }
        }

        /// <summary>
        /// Обработчик кнопки Тест
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Test(object sender, RoutedEventArgs e)
        {
            string URL = URLTextBox.Text;
            if (string.IsNullOrWhiteSpace(URL))
            {
                MessageBox.Show("Вы не вставили ссылку!");
                return;
            }

            var driver = Driver.GetInstance();

            int perPageCount = await Parser.GetArticlesPerPageAsync(driver, URL);
            int totalCount = await Parser.GetArticlesCountAsync(driver, URL);
            int pages = (int)Math.Floor((decimal)totalCount / perPageCount);

            SetMaximumSlider(pages);
            DisableTestButton();
            EnableSlider();
            EnableStartButton();
        }
    }
}
