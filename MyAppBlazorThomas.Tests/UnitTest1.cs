using Bunit;
using MyAppBlazorThomas.Pages;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class LoginTests
{
    [Fact]
    public void Login_AfficheDeuxInputsEtUnBouton()
    {
        using var ctx = new BunitContext();
        var cut = ctx.Render<Login>();

        Assert.Equal(2, cut.FindAll("input").Count);
        Assert.Contains("Login", cut.Markup);
    }
    [Fact]
    public void Login_BonIdentifiants_NavigueVersKaamelott()
    {
        using var ctx = new BunitContext();
        var nav = ctx.Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/login");

        var cut = ctx.Render<Login>();

        cut.Find("input[placeholder='Login']").Change("admin");
        cut.Find("input[type='password']").Change("1234");

        cut.Find("form").Submit();

        Assert.EndsWith("/kaamelott", nav.Uri);
    }
    [Fact]
    public void Login_PasswordInput_EstDeTypePassword()
    {
        using var ctx = new BunitContext();
        var cut = ctx.Render<Login>();

        var passwordInput = cut.Find("input[type='password']");
        Assert.NotNull(passwordInput);
    }
}
