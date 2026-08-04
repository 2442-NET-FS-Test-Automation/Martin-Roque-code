using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

//XPath stands for XML Path Language. Query elements as if ther were filepaths on the DOM

public class XPathTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public XPathTests()
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
    public void RelativeXPath_MatchesByAttribute()
    {
        var cards = _driver.FindElements(By.XPath("//article"));

        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void XPathFunctions_MatchOnText()
    {
        var cleanCode = _driver.FindElement(By.XPath("//h3/a[contains(text(), 'Clean')]"));
        cleanCode.Text.Should().Be("Clean Code");

        var skus = _driver.FindElements(By.XPath("//dd[starts-with(text(), 'BK-')]"));

        skus.Should().HaveCount(3);
    }

    [Fact]
    public void XPathes_WalkUpAndSideways()
    {
        var cardOfCleanCode = _driver.FindElement(
            By.XPath("//a[text()='Clean Code']/ancestor::article")
        );

        cardOfCleanCode.GetAttribute("class").Should().Be("cards");

        var firstSku = _driver.FindElement(
            By.XPath("//dt[text()='SKU']/following-sibling::dd[1]")
        );

        firstSku.Text.Should().Be("BK-001");
    }

    [Fact]
    public void AbsoluteXPath_WorksToday()
    {
        var h1 = _driver.FindElement(
            By.XPath("/html/body/div/div/header/h1")
        );

        h1.Text.Should().Be("Library");
    }
}