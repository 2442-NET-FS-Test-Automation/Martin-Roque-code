using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

//Locator demo- this is how your tests can navigate your SPA.

public class LocatorTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public LocatorTests()
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
    public void ByTagName_FirstTheheader()
    {
        _driver.FindElement(By.TagName("h1")).Text.Should().Be("Library");
    }

    [Fact]
    public void ByClassName_FindsEveryCard()
    {
        var cards = _driver.FindElements(By.ClassName("cards"));
        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void ByCssSelector_ComposesStructureAndClass()
    {
        var firstTitleLink = _driver.FindElement(By.CssSelector("article.card h3 a"));
        firstTitleLink.Text.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ByLinkTest_FindsAnchorsByWhatUserReads()
    {
        _driver.FindElement(By.LinkText("About")).TagName.Should().Be("a");
        _driver.FindElement(By.PartialLinkText("Cata")).Text.Should().Be("Catalog");
    }
}