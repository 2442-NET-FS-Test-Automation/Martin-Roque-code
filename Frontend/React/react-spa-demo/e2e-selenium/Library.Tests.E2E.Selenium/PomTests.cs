using FluentAssertions;
using Library.Tests.E2E.PageObjects;

namespace Library.Tests.E2E.Selenium;

public class PomTests : E2ETestBase
{
    [Fact]
    public void Filter_ThroughThePageObject()
    {
        var catalog = new CatalogPage(Driver).Visit().Search("clean");

        catalog.CardCount.Should().Be(1);
        catalog.FirstTitle.Should().Be("Clean Code");
    }

    [Fact]
    public void Sort_ThroughThePageObject()
    {
        var catalog = new CatalogPage(Driver).Visit().ToggleSort();

        catalog.FirstTitle.Should().Be("The Pragmatic Programmer");
    }

    [Fact]
    public void SignIn_AcrossPages()
    {
        Guarded(() =>
        {
            var catalog = new LoginPage(Driver).Visit().SignInAs("ada", "pass123!");

            catalog.SignedInUser.Should().Be("ada (admin)");
        });
    }
}