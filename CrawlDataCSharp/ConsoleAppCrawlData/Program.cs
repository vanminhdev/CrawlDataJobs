using ConsoleAppCrawlData.Configs;
using ConsoleAppCrawlData.Services;
using ConsoleAppCrawlData.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ConsoleAppCrawlData
{
    internal class Program
    {
        static void CrawlItViec(IConfiguration configuration, JobsService jobsService)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized");
            IWebDriver driver = new ChromeDriver(chromeOptions);
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
                try
                {
                    driver.Navigate().GoToUrl($"https://itviec.com/viec-lam-it?locale=vi&page={i}");
                    var byJobsElement = By.XPath("//*[@id=\"jobs\"]/div[1]");
                    wait.Until(drv => drv.FindElement(byJobsElement));
                    var jobElements = driver.FindElement(byJobsElement);
                    var jobDivElements = jobElements.FindElements(By.ClassName("job"));
                    int indexDiv = 0;
                    foreach (var jobDivElement in jobDivElements)
                    {
                        indexDiv++;
                        //var test = jobDivElement.GetAttribute("innerHTML");
                        var byDivJob = By.XPath($"//div[@id=\"jobs\"]/div[@class=\"first-group\"]/div[{indexDiv}]");
                        var jobDevElementNew = driver.FindElement(byDivJob);
                        var test2 = jobDevElementNew.GetAttribute("innerHTML");
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", jobDivElement); //scroll để có thể click vào toạ độ của element
                        Thread.Sleep(300);
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(byDivJob));
                        jobDevElementNew.Click();
                        if (driver.WindowHandles.Count > 1)
                        {
                            driver.SwitchTo().Window(driver.WindowHandles[1]);
                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles[0]);
                        }

                        //Thread.Sleep(1000);
                        Thread.Sleep(400);
                        var byJobTitle = By.XPath("//h1[@class=\"job-details__title\"]");
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(byJobTitle));
                        var jobTitleElement = driver.FindElement(byJobTitle);
                        string jobTitle = jobTitleElement.Text?.Trim();

                        var byTags = By.XPath("//div[@class=\"job-details__tag-list\"]");
                        var tagsElements = driver.FindElement(byTags);
                        wait.Until(drv => drv.FindElement(byTags));
                        //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", tagsElements);
                        //string innerHtml = tagsElements.GetAttribute("innerHTML");
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
                catch
                {
                    continue;
                }
            }
        }

        static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            IConfiguration configuration = host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            JobsService jobsService = host.Services.GetService(typeof(JobsService)) as JobsService;
            CrawlItViec(configuration, jobsService);
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
                    services.Configure<JobStoreDatabaseSettings>(hostContext.Configuration.GetSection("JobStoreDatabase"));
                    services.AddScoped<JobsService>();
                });
    }
}