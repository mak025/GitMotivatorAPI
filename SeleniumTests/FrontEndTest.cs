using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace SeleniumTests
{
    public class FrontEndTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly ITestOutputHelper _output;
        private readonly HttpClient _http = new HttpClient();

        private const string ApiURL = "https://localhost:7223/";
        //private const string FrontEndURL = "file:///C:/3.Semester/3.SemesterEksamenGitHubMotivator/gitmotivator/dist/index.html";
        private const string FrontEndURL = "localhost:3000";


        public FrontEndTest(ITestOutputHelper output)
        {
            _output = output;

            var service = FirefoxDriverService.CreateDefaultService(@"C:\3. Semester\3.SemesterEksamenGitHubMotivator\FireFoxDriver", "geckodriver.exe");
            service.HideCommandPromptWindow = true;

            var options = new FirefoxOptions();
            options.AcceptInsecureCertificates = true;
            options.BinaryLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe"; // add this line

            _driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(60));
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }
        [Fact]
        public void LoginTest()
        {
            var loginName = "Glargil";
            var password = "10Mortsde!";
            _driver.Navigate().GoToUrl(FrontEndURL);
            
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            //wait until we find the element to prevent delays failing the test
            wait.Until(d => d.FindElements(By.CssSelector("button[class='w-full flex items-center justify-center gap-3 px-6 py-4 text-base font-black text-premium-bg bg-premium-green hover:bg-premium-green-dark rounded-2xl transition-all shadow-[0_0_20px_rgba(62,228,124,0.3)] active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed']")));
            
            //store the button as a variable
            var button = _driver.FindElement(By.CssSelector("button[class='w-full flex items-center justify-center gap-3 px-6 py-4 text-base font-black text-premium-bg bg-premium-green hover:bg-premium-green-dark rounded-2xl transition-all shadow-[0_0_20px_rgba(62,228,124,0.3)] active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed']"));
            
            button.Click();

            var currentUrl = _driver.Url;

            Assert.NotEqual(FrontEndURL, currentUrl);
            
            //wait.Until(d => d.FindElement(By.Id("login_field")));


            //wait.Until(d => d.FindElement(By.Id("password")));

        }

        public void Dispose()
        {
            _http.Dispose();
            try { _driver?.Quit(); }
            catch { }
            finally { _driver?.Dispose(); }
        }
    }
}
