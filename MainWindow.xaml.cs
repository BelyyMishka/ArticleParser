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
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик кнопки Начать парсинг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartParsing(object sender, RoutedEventArgs e)
        {
            string URL = URLTextBox.Text;
            if(string.IsNullOrWhiteSpace(URL))
            {
                MessageBox.Show("Вы не вставили ссылку!");
                return;
            }

            if(!Parser.IsFirstPage(URL))
            {
                MessageBox.Show("Перейдите на первую страницу!");
                return;
            }

            var driver = Driver.GetInstance();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            ToggleControls(sender);

            int count = await Parser.GetArticlesPerPageAsync(driver, URL);
            int pages = await Parser.GetPagesAsync(driver);

            pages = 1;
            count = 1;
            SetMaximun(count * pages);

            for (int i = 0; i < pages; i++ )
            {
                await Parser.ParsePerPageAsync(driver, path, count);
                Increase(count);
                if(i == pages - 1)
                {
                    break;
                }
                await Parser.GoToNextPageAsync(driver, i + 2);
            }
            
            Driver.Quit();
            MessageBox.Show("Парсинг завершен!");
            ToggleControls(sender);
            Environment.Exit(0);
        }

        /// <summary>
        /// Метод состояния кнопки
        /// </summary>
        /// <param name="sender"></param>
        private void SwitchButtonState(object sender)
        {
            Button button = (Button)sender;
            button.IsEnabled = !(button.IsEnabled);
        }

        /// <summary>
        /// Метод состояния поля ввода
        /// </summary>
        private void SwitchTextBoxState()
        {
            URLTextBox.IsEnabled = !(URLTextBox.IsEnabled);
        }

        /// <summary>
        /// Метод для переключения состояний элементов управления
        /// </summary>
        /// <param name="sender"></param>
        private void ToggleControls(object sender)
        {
            SwitchButtonState(sender);
            SwitchTextBoxState();
        }

        /// <summary>
        /// Метод для установки максимального значения шкалы прогресса
        /// </summary>
        /// <param name="max">Максимальное значение</param>
        private void SetMaximun(int max)
        {
            ProgressBar.Maximum = max;
        }

        /// <summary>
        /// Метод для увелечения шкалы прогресса на значение
        /// </summary>
        /// <param name="value">Значение для увелечения</param>
        private void Increase(int value)
        {
            ProgressBar.Value += value;
        }

        /// <summary>
        /// Обработчик кнопки Остановить парсинг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopParsing(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
