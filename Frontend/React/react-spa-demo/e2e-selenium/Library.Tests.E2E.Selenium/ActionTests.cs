using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace Library.Tests.E2E.Selenium;

public class ActionTests : IDisposable
{
    private readonly ChromeDriver _driver;

    private static string WidgetUrl =>
        new Uri(Path.Combine(AppContext.BaseDirectory, "TestPages",
        "widgets.html")).AbsoluteUri;

    public ActionTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--windows.size=1280,900");

        _driver = new ChromeDriver();

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

        _driver.Navigate().GoToUrl(WidgetUrl);
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void Hover_RevealsTheMenu()
    {
        var menu = _driver.FindElement(By.Id("hover-menu"));
        menu.Displayed.Should().BeFalse();

        new Actions(_driver)
            .MoveToElement(_driver.FindElement(By.Id("hover-zone")))
            .Perform();

        menu.Displayed.Should().BeTrue();
        menu.Text.Should().Be("Now you see the menu");
    }

    [Fact]
    public void DoubleClick_FiresTheDblClickEvent()
    {
        new Actions(_driver)
            .DoubleClick(_driver.FindElement(By.Id("dbl-btn")))
            .Perform();

        _driver.FindElement(By.Id("dbl-count")).Text.Should().Be("1");
    }

    [Fact]
    public void KeyBoardChord_TypesUppercaseWithShift()
    {
        var input = _driver.FindElement(By.Id("keys-input"));

        new Actions(_driver)
            .Click(input)
            .KeyDown(Keys.Shift)
            .SendKeys("ada")
            .KeyUp(Keys.Shift)
            .Perform();

        input.GetAttribute("value").Should().Be("ADA");
    }
}