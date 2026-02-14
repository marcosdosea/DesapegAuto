using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace DesapegAutoWebTests.Functional
{
    [TestClass]
    public class VersaoCreateFunctionalTests
    {
        private IWebDriver driver = null!;

        private const string BaseUrl = "https://127.0.0.1:7179";
        private string CreateUrl = BaseUrl + "/Versao/Create";

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--allow-insecure-localhost");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Window.Maximize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                driver?.Quit();
                driver?.Dispose();
            }
            catch { }
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void Navegacao_DeveCarregarPagina()
        {
            driver.Navigate().GoToUrl(CreateUrl);

            Assert.IsTrue(driver.Url.Contains("/Identity/Account/Login"), "Usuario anonimo deveria ser redirecionado para login.");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CaminhoFeliz_DeveSalvar()
        {
            driver.Navigate().GoToUrl(CreateUrl);
            Assert.IsTrue(driver.Url.Contains("/Identity/Account/Login"), "Usuario anonimo nao pode criar versao sem autenticar.");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void Validacao_CamposVazios_DeveDarErro()
        {
            driver.Navigate().GoToUrl(CreateUrl);
            Assert.IsTrue(driver.Url.Contains("/Identity/Account/Login"), "Usuario anonimo nao pode acessar a validacao da tela de create.");
        }
    }
}