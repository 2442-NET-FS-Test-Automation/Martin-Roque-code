using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Library.Tests.E2E.PageObjects;

public class LoginPage
{
    private readonly IWebDriver _driver;

    private static readonly By Username = By.CssSelector("form.login input:not([type='password'])");
    private static readonly By Password = By.CssSelector("form.login input[type='password']");
    private static readonly By Submit = By.CssSelector("form.login button[type='submit']");

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public LoginPage Visit()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/login");
        return this;
    }

    public CatalogPage SignInAs(string username, string password)
    {
        _driver.FindElement(Username).SendKeys(username);
        _driver.FindElement(Password).SendKeys(password);
        _driver.FindElement(Submit).Click();

        new WebDriverWait(_driver, TimeSpan.FromSeconds(5))
            .Until(d => d.FindElements(By.CssSelector(".auth-box span")).Count > 0);

        return new CatalogPage(_driver);
    }
}