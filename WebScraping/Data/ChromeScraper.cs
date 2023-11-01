using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using Microsoft.VisualBasic.FileIO;
using WebScraping.DTO;
using WebScraping.Data;
using System.Collections.ObjectModel;

namespace WebScraping
{
    public class ChromeScraper
    {
        private ChromeDriver driver;

        public void BasicOptions()
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(options);
        }

        public void CreateDriver()
        {
            var options = new ChromeOptions();

            options.AddArgument("--start-maximized");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--log-level=3");

            driver = new ChromeDriver(options);
        }

        public void Navigate(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public void Click(string xPath)
        {

            var control = driver.FindElement(By.XPath(xPath));
            control.Click();
        }
        public bool ElementExists(string path)
        {
            bool control = driver.FindElements(By.XPath(path)).Count() > 0 ? true : false;
            return control;
        }

        public void WaitForElement(string xPath, int NumOfSeconds)
        {

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(NumOfSeconds));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xPath)));
        }

        public ReadOnlyCollection<IWebElement> SelectElements(string xPath)
        {
            var elements = driver.FindElements(By.XPath(xPath));
            return elements;
        }

        public IWebElement SelectElement(string xPath)
        {
            var element = driver.FindElement(By.XPath(xPath));
            return element;
        }

        public void ExcludeSwitch()
        {
            var options = new ChromeOptions();

            options.AddExcludedArgument("disable-popup-blocking");

            driver = new ChromeDriver(options);
        }
        public void LogsToConsole(string strText)
        {
            var stringWriter = new StringWriter();
            var originalOutput = Console.Out;
            Console.SetOut(stringWriter);

            var service = ChromeDriverService.CreateDefaultService();

            //service.LogToConsole = true;

            driver = new ChromeDriver(service);

            //Assert.IsTrue(stringWriter.ToString().Contains("Starting ChromeDriver"));
            Console.SetOut(originalOutput);
            stringWriter.Dispose();
        }

        public void CloseDriver()
        {
            driver.Quit();
        }
    }
}
