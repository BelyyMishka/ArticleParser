using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private async void StartParsing(object sender, RoutedEventArgs e)
        {
            Reset();
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
            int count;
            int pages;

            ToggleControls(sender);

            try
            {
                count = await Parser.GetArticlesPerPageAsync(driver, URL);
                pages = await Parser.GetPagesAsync(driver);
            }
            catch
            {
                Driver.Quit();
                MessageBox.Show("Недействительная ссылка!");
                ToggleControls(sender);
                return;
            }

            pages = 1;
            count = 5;
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

            return;
        }

        private void SwitchButtonState(object sender)
        {
            Button button = (Button)sender;
            button.IsEnabled = !(button.IsEnabled);
        }

        private void SwitchTextBoxState()
        {
            URLTextBox.IsEnabled = !(URLTextBox.IsEnabled);
        }

        private void ToggleControls(object sender)
        {
            SwitchButtonState(sender);
            SwitchTextBoxState();
        }

        private void SetMaximun(int max)
        {
            ProgressBar.Maximum = max;
        }

        private void Reset()
        {
            ProgressBar.Value = 0;
        }

        private void Increase(int value)
        {
            ProgressBar.Value += value;
        }

        private void StopParsing(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
