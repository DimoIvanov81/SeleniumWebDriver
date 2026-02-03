using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace TestProject1
{
    [TestFixture]
    public class TestCalculator
    {
        private IWebDriver driver;

        private IWebElement textBoxFirstNum;
        private IWebElement textBoxSecondNum;
        private IWebElement dropDownOperation;
        private IWebElement calcBtn;
        private IWebElement resetBtn;
        private IWebElement divResult;

        [OneTimeSetUp]
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
            driver.Navigate().GoToUrl("https://calculatorhtml.onrender.com/");

            textBoxFirstNum = driver.FindElement(By.Id("number1"));
            dropDownOperation = driver.FindElement(By.Id("operation"));
            textBoxSecondNum = driver.FindElement(By.Id("number2"));
            calcBtn = driver.FindElement(By.Id("calcButton"));
            resetBtn = driver.FindElement(By.Id("resetButton"));
            divResult = driver.FindElement(By.Id("result"));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }

        private void PerformCalculation(string firstNumber, string operation, string secondNumber, string expectedResult)
        {
            resetBtn.Click();

            if (!string.IsNullOrEmpty(firstNumber))
                textBoxFirstNum.SendKeys(firstNumber);

            if (!string.IsNullOrEmpty(secondNumber))
                textBoxSecondNum.SendKeys(secondNumber);

            if (!string.IsNullOrEmpty(operation))
                new SelectElement(dropDownOperation).SelectByText(operation);

            calcBtn.Click();

            Assert.That(divResult.Text, Is.EqualTo(expectedResult));
        }

        [TestCase("5", "+ (sum)", "10", "Result: 15")]
        [TestCase("3.5", "- (subtract)", "1.2", "Result: 2.3")]
        [TestCase("2e2", "* (multiply)", "1.5", "Result: 300")]
        [TestCase("5", "/ (divide)", "0", "Result: Infinity")]
        [TestCase("invalid", "+ (sum)", "10", "Result: invalid input")]
        public void TestNumberCalculator(string firstNumber, string operation, string secondNumber, string expectedResult)
        {
            PerformCalculation(firstNumber, operation, secondNumber, expectedResult);
        }
    }
}
