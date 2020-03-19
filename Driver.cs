using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ArticleParser
{
    static class Driver
    {
        private static ChromeDriver driver;

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

        public static void Quit()
        {
            driver.Quit();
        }


    }
}
