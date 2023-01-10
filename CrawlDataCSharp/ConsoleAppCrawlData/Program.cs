using ConsoleAppCrawlData.Configs;
using ConsoleAppCrawlData.Models;
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
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace ConsoleAppCrawlData
{
    internal class Program
    {
        static void CrawlItViec(IConfiguration configuration, JobsService jobsService)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized"); //full screen
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

                driver.Navigate().GoToUrl($"https://itviec.com/viec-lam-it?locale=vi&page={i}");
                var byJobsElement = By.XPath("//*[@id=\"jobs\"]/div[1]");
                wait.Until(drv => drv.FindElement(byJobsElement));
                var jobElements = driver.FindElement(byJobsElement);
                var jobDivElements = jobElements.FindElements(By.ClassName("job"));
                int indexDiv = 0;
                foreach (var jobDivElement in jobDivElements)
                {
                    indexDiv++;
                    try
                    {
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
                        List<string> skills = new();
                        foreach (var tagsElement in tagsElements.FindElements(By.TagName("span")))
                        {
                            string skillStr = tagsElement.GetAttribute("innerHTML").NormalizeString();
                            skills.Add(skillStr);
                        }

                        //salary
                        var salaryElement = driver.FindElement(By.XPath("//div[@class=\"job-details__overview\"]/div[2]/div"));
                        var salary = salaryElement.Text.NormalizeString();
                        int? startSalary = null;
                        int? endSalary = null;
                        try
                        {
                            var match = Regex.Match(salary, @"([\d]+)\s*-\s*([\d,]+)");
                            if (match.Success)
                            {
                                string match1 = match.Groups[1].Value.Replace(",", "");
                                string match2 = match.Groups[2].Value.Replace(",", "");
                                startSalary = int.Parse(match1);
                                endSalary = int.Parse(match2);
                            }
                        }
                        catch
                        {
                        }

                        var byAddressElement = By.XPath("//div[@class=\"job-details__overview\"]/div[@class=\"svg-icon\"]/div[@class=\"svg-icon__text\"]/span");
                        wait.Until(drv => drv.FindElement(byAddressElement));
                        var addressElement = driver.FindElement(byAddressElement);
                        var addresss = addressElement.Text.NormalizeString();

                        //overview
                        var companyNameElement = driver.FindElement(By.ClassName("search-page-employer-overview__name"));
                        var companyName = companyNameElement.Text.NormalizeString();

                        var companyTypeElement = driver.FindElement(By.XPath("//div[@class=\"search-page-employer-overview__characteristics\"]/div[1]/div"));
                        var companyType = companyTypeElement.Text.NormalizeString();

                        var companySizeElement = driver.FindElement(By.XPath("//div[@class=\"search-page-employer-overview__characteristics\"]/div[2]/div"));
                        var companySize = companySizeElement.Text.NormalizeString();

                        int? startCompanySize = null;
                        int? endCompanySize = null;
                        try
                        {
                            var match = Regex.Match(companySize, @"([\d]+)\s*-\s*([\d]+)");
                            if (match.Success)
                            {
                                string match1 = match.Groups[1].Value.Replace(",", "");
                                string match2 = match.Groups[2].Value.Replace(",", "");
                                startCompanySize = int.Parse(match1);
                                endCompanySize = int.Parse(match2);
                            }
                        }
                        catch
                        {
                        }

                        var workingTimeElement = driver.FindElement(By.XPath("//div[@class=\"search-page-employer-overview__characteristics\"]/div[3]/div"));
                        var workingTime = workingTimeElement.Text.NormalizeString();

                        var nationElement = driver.FindElement(By.XPath("//div[@class=\"search-page-employer-overview__characteristics\"]/div[4]/div"));
                        var nation = nationElement.Text.NormalizeString();

                        var overtimeElement = driver.FindElement(By.XPath("//div[@class=\"search-page-employer-overview__characteristics\"]/div[5]/div"));
                        var overtime = overtimeElement.Text.NormalizeString();

                        jobsService.Insert(new Jobs
                        {
                            Link = driver.Url,
                            JobName = jobTitle,
                            Skills = skills,
                            PostTime = DateTime.Now,
                            WorkingTime = workingTime,
                            Overtime = overtime,
                            StartSalary = startSalary,
                            EndSalary = endSalary,
                            Experience = null,
                            Positions = null,
                            Company = new Company
                            {
                                CompanyName = companyName,
                                Address = addresss,
                                CompanyType = companyType,
                                StartCompanySize = startCompanySize,
                                EndCompanySize = endCompanySize,
                                Nation = nation,
                            }
                        });
                    }
                    catch
                    {
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            IConfiguration configuration = host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            JobsService jobsService = host.Services.GetService(typeof(JobsService)) as JobsService;
            //jobsService.DeleteAll();
            //CrawlItViec(configuration, jobsService);
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