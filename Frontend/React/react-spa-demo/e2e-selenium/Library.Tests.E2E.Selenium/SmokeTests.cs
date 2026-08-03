using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Testt.E2E.Selenium;

public class SmokeTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public SmokeTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--windows.size=1280,900");

        _driver = new ChromeDriver();

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void OpeningTheSpa_ShowsTitleAndHeading()
    {
        //Act - real navigation in real browser
        _driver.Navigate().GoToUrl("http://localhost:5173/");

        //Assert 
        _driver.Title.Should().Be("Library - catalog");
        _driver.FindElement(By.TagName("h1")).Text.Should().Be("Library");
    }

    [Fact]
    public void Catalog_RenderBookCards_FromTheLiveApi()
    {
        //Act - real navigation in real browser
        _driver.Navigate().GoToUrl("http://localhost:5173/");

        //Assert
        var cards = _driver.FindElements(By.CssSelector("article.card"));
        cards.Should().NotBeEmpty();
    }
}