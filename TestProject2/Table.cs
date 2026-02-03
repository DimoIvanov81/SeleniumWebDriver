using System.Collections.ObjectModel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject2
{
    [TestFixture]
    public class WorkingWithWebTable
    {
        private IWebDriver driver;

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void TestExtractProductInformation()
        {
            driver.Navigate().GoToUrl("http://practice.bpbonline.com/");

            IWebElement productTable = driver.FindElement(By.XPath("//*[@id='bodyContent']/div/div[2]/table"));

            // ВАЖНО: точка отпред, за да търси редовете вътре в таблицата, не в целия документ
            ReadOnlyCollection<IWebElement> tableRows = productTable.FindElements(By.XPath(".//tbody/tr"));

            string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "productinformation.csv");

            if (File.Exists(path))
                File.Delete(path);

            foreach (IWebElement trow in tableRows)
            {
                // пак с ".//" за вътре в реда
                ReadOnlyCollection<IWebElement> tableCols = trow.FindElements(By.XPath(".//td"));

                foreach (IWebElement tcol in tableCols)
                {
                    string data = tcol.Text?.Trim();
                    if (string.IsNullOrWhiteSpace(data))
                        continue;

                    string[] productinfo = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    // Пази се: понякога няма 2 реда
                    if (productinfo.Length < 2)
                        continue;

                    string printProductinfo = productinfo[0].Trim() + "," + productinfo[1].Trim() + Environment.NewLine;
                    File.AppendAllText(path, printProductinfo);
                }
            }

            Assert.IsTrue(File.Exists(path), "CSV file was not created");
            Assert.IsTrue(new FileInfo(path).Length > 0, "CSV file is empty");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
