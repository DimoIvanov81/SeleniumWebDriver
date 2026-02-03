using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace TestProject3
{
    [TestFixture]
    public class WorkingWithDropDown
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

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }

        [Test]
        public void TestSelectFromDropDown()
        {
            driver.Navigate().GoToUrl("http://practice.bpbonline.com/");

            string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "manufacturer.txt");
            if (File.Exists(path))
                File.Delete(path);

            // IMPORTANT: locate the real <select>, not an <input> with the same name
            SelectElement manufDropdown =
                new SelectElement(driver.FindElement(By.CssSelector("select[name='manufacturers_id']")));

            // options in the dropdown
            IList<IWebElement> allManufacturers = manufDropdown.Options;

            // manufacturer names (skip "Please Select")
            List<string> manufNames = new List<string>();
            foreach (IWebElement option in allManufacturers)
                manufNames.Add(option.Text);

            if (manufNames.Count > 0)
                manufNames.RemoveAt(0);

            foreach (string mname in manufNames)
            {
                // select manufacturer
                manufDropdown.SelectByText(mname);

                // re-find dropdown after navigation/refresh
                manufDropdown =
                    new SelectElement(driver.FindElement(By.CssSelector("select[name='manufacturers_id']")));

                if (driver.PageSource.Contains("There are no products available in this category."))
                {
                    File.AppendAllText(path, $"The manufacturer {mname} has no products{Environment.NewLine}");
                    continue;
                }

                // product table
                IWebElement productTable = driver.FindElement(By.ClassName("productListingData"));
                File.AppendAllText(path,
                    $"{Environment.NewLine}{Environment.NewLine}The manufacturer {mname} products are listed--{Environment.NewLine}");

                // IMPORTANT: search inside the table only
                ReadOnlyCollection<IWebElement> rows = productTable.FindElements(By.XPath(".//tbody/tr"));

                foreach (IWebElement row in rows)
                    File.AppendAllText(path, row.Text + Environment.NewLine);
            }

            // optional sanity checks
            Assert.IsTrue(File.Exists(path), "Output file was not created.");
            Assert.IsTrue(new FileInfo(path).Length > 0, "Output file is empty.");
        }
    }
}
