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

            var driver = Driver.GetInstance();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            ToggleControls(sender);

            int perPageCount = await Parser.GetArticlesPerPageAsync(driver, URL);
            int totalCount = await Parser.GetArticlesCountAsync(driver, URL);
            int pages = (int)Math.Floor((decimal)totalCount / perPageCount);

            SetMaximun(totalCount);

            for (int i = 0; i < pages; i++ )
            {
                await Parser.ParsePerPageAsync(driver, path, perPageCount);
                Increase(perPageCount);
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
            SwitchStopButtonState();
        }

        /// <summary>
        /// Метод состояния кнопки Остановить парсинг
        /// </summary>
        private void SwitchStopButtonState()
        {
            Button button = StopButton;
            button.IsEnabled = !(button.IsEnabled);
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
            Driver.GetInstance().Close();
            Driver.GetInstance().Quit();
            Environment.Exit(0);
        }
    }
}
