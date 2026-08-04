using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class ScreenShots : IDisposable
{
    private readonly ChromeDriver _driver;

    public ScreenShots()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--windows.size=1280,900");

        _driver = new ChromeDriver();

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

        _driver.Navigate().GoToUrl("http://localhost:5173/");

        _driver.FindElements(By.CssSelector("article.card")).Should().NotBeEmpty();
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void FullPage_SaveAPng()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "shots", "catalog-page.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        _driver.GetScreenshot().SaveAsFile(path);

        File.Exists(path).Should().BeTrue();
    }

    [Fact]
    public void SingleElement_SavesItsOwnPng()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "shots", "first-card.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var card = _driver.FindElement(By.CssSelector("article.card"));

        ((ITakesScreenshot)card).GetScreenshot().SaveAsFile(path);

        File.Exists(path).Should().BeTrue();
    }
}