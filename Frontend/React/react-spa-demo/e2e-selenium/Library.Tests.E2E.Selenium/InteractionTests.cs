using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class InteractionTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public InteractionTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--windows.size=1280,900");

        _driver = new ChromeDriver();

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

        _driver.Navigate().GoToUrl("http://localhost:5173/");
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void LoginForm_SignsIn_ThroughTheUi()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/login");

        var username = _driver.FindElement
        (By.CssSelector("form.login input:not([type='password'])"));

        var password = _driver.FindElement(
            By.CssSelector("form.login input[type='password']")
        );

        var submint = _driver.FindElement(By.CssSelector("form.login button[type='submit']"));

        username.SendKeys("ada");
        password.SendKeys("pass123!");
        submint.Click();

        var who = _driver.FindElement(By.CssSelector(".auth-box span"));
        who.Text.Should().Be("ada (admin)");
    }

    [Fact]
    public void SendKeyAndClear_DriveAControl()
    {
        var search = _driver.FindElement(By.CssSelector("input[type='search']"));

        search.GetAttribute("placeholder").Should().Be("Filter by name...");

        search.SendKeys("clean");
        search.GetAttribute("value").Should().Be("clean");

        _driver.FindElements(By.CssSelector("article.card")).Should().HaveCount(1);

        search.Clear();
        search.GetAttribute("value").Should().Be("");
    }

    [Fact]
    public void DisplayAndEnabled_ReadElementState()
    {
        var heading = _driver.FindElement(By.TagName("h2"));

        heading.Displayed.Should().BeTrue();
        heading.Text.Should().Be("Catalog");

        _driver.FindElement(By.CssSelector(".toolbar button")).Enabled.Should().BeTrue();
    }
}