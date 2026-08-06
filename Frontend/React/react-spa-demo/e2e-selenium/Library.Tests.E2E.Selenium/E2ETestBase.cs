using System.Runtime.CompilerServices;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public abstract class E2ETestBase : IDisposable
{
    protected ChromeDriver Driver { get; }

    protected static string WidgetUrl =>
    new Uri(Path.Combine(AppContext.BaseDirectory, "TestPages",
        "widgets.html")).AbsoluteUri;

    protected E2ETestBase()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--windows.size=1280,900");

        Driver = new ChromeDriver();
    }

    //Screenshot capture-on-failure
    protected void Guarded(Action act, [CallerMemberName] string testName = "")
    {
        try
        {
            act();
        }
        catch
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "shots", $"FAILED{testName}.png"
            );
            Driver.GetScreenshot().SaveAsFile(path);

            throw;
        }
    }

    public void Dispose()
    {
        Driver.Quit();
    }
}