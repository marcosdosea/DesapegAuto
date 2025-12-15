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

            Assert.IsTrue(driver.Url.Contains("/Versao/Create"), "Não carregou a página de Create.");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CaminhoFeliz_DeveSalvar()
        {
            driver.Navigate().GoToUrl(CreateUrl);

            // Preenche os campos (Ajuste os IDs se seu HTML for diferente)
            driver.FindElement(By.Id("Id")).Clear();
            driver.FindElement(By.Id("Id")).SendKeys(new Random().Next(100, 999).ToString());

            driver.FindElement(By.Id("Nome")).Clear();
            driver.FindElement(By.Id("Nome")).SendKeys("Versão Teste Manual");

            driver.FindElement(By.Id("IdModelo")).Clear();
            driver.FindElement(By.Id("IdModelo")).SendKeys("1"); // Garanta que tem Modelo 1 no banco

            // Clica em Salvar
            driver.FindElement(By.CssSelector("input[type='submit']")).Click();

            // Se salvou, deve ter saído da tela de Create
            bool saiuDaTela = !driver.Url.EndsWith("/Versao/Create");
            Assert.IsTrue(saiuDaTela, "Deveria ter redirecionado após salvar.");
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void Validacao_CamposVazios_DeveDarErro()
        {
            driver.Navigate().GoToUrl(CreateUrl);

            // Limpa e tenta salvar vazio
            driver.FindElement(By.Id("Id")).Clear();
            driver.FindElement(By.Id("Nome")).Clear();
            driver.FindElement(By.Id("IdModelo")).Clear();

            driver.FindElement(By.CssSelector("input[type='submit']")).Click();

            // Deve continuar na mesma tela e mostrar erro
            Assert.IsTrue(driver.Url.Contains("/Versao/Create"));
            var erros = driver.FindElements(By.ClassName("field-validation-error"));
            Assert.IsTrue(erros.Count > 0, "Faltou a mensagem de erro.");
        }
    }
}