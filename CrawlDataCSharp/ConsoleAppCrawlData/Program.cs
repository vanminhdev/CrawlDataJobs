using ConsoleAppCrawlData.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ConsoleAppCrawlData
{
    internal class Program
    {
        static void CrawlItViec(IConfiguration configuration)
        {
            IWebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            driver.Navigate().GoToUrl("https://itviec.com/sign_in");
            wait.Until(drv => drv.FindElement(By.Id("user_email")));

            string username = configuration.GetValue<string>("Login:ItViec:Username");
            string password = configuration.GetValue<string>("Login:ItViec:Password");

            var inputEmailElement = driver.FindElement(By.Id("user_email"));
            inputEmailElement.SendKeys(username);
            var inputPasswordElement = driver.FindElement(By.Id("user_password"));
            inputPasswordElement.SendKeys(password);

            var buttonSubmit = driver.FindElement(By.XPath("//button[@type=\"submit\"]"));
            buttonSubmit.Click();

            driver.Navigate().GoToUrl("https://itviec.com/viec-lam-it");

            var element = driver.FindElement(By.XPath("//*[@id=\"jobs\"]/div[2]/ul/li[4]/a"));
            int maxPage = int.Parse(element.Text.Trim());

            for (int i = 1; i <= maxPage; i++)
            {
                driver.Navigate().GoToUrl($"https://itviec.com/viec-lam-it?locale=vi&page={i}");
                var byJobsElement = By.XPath("//*[@id=\"jobs\"]/div[1]");
                wait.Until(drv => drv.FindElement(byJobsElement));
                var jobElements = driver.FindElement(byJobsElement);
                var jobDivElements = jobElements.FindElements(By.ClassName("job"));
                foreach (var jobDivElement in jobDivElements)
                {
                    var test = jobDivElement.GetAttribute("innerHTML");
                    wait.Until(ExpectedConditions.ElementToBeClickable(jobDivElement));
                    jobDivElement.Click();
                    Thread.Sleep(500);
                    wait.Until(drv => drv.FindElement(By.XPath("//h1[@class=\"job-details__title\"]")));
                    var nameElement = driver.FindElement(By.XPath("//h1[@class=\"job-details__title\"]"));
                    string name = nameElement.Text.Trim();

                    var tagsElements = driver.FindElement(By.XPath("//div[@class=\"job-details__tag-list\"]"));
                    string innerHtml = tagsElements.GetAttribute("innerHTML");
                    foreach (var tagsElement in tagsElements.FindElements(By.TagName("span")))
                    {
                        string skillStr = tagsElement.GetAttribute("innerHTML").NormalizeString();
                    }

                    var byAddressElement = By.XPath("//div[@class=\"job-details__overview\"]/div[@class=\"svg-icon\"]/div[@class=\"svg-icon__text\"]/span");
                    wait.Until(drv => drv.FindElement(byAddressElement));
                    var addressElement = driver.FindElement(byAddressElement);
                    var addresss = addressElement.Text.NormalizeString();
                }
            }
        }

        static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            IConfiguration configuration = host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            CrawlItViec(configuration);
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json");
                    if (File.Exists("appsettings.Development.json"))
                    {
                        app.AddJsonFile("appsettings.Development.json");
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    string connectionString = hostContext.Configuration.GetConnectionString("Default");
                });
    }
}