using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TakedownTCG.Core.Abstractions;
using TakedownTCG.Core.Infrastructure.Persistence.UserAccounts;
using TakedownTCG.Core.Services.UserAccounts;

namespace TakedownTCG.Web.Tests;

public sealed class WebEndpointsTests
{
    [Fact]
    public async Task HomePage_RendersUniqueMobileSection()
    {
        await using TestAppFactory factory = new();
        using HttpClient client = factory.CreateClient();

        string html = await client.GetStringAsync("/");

        Assert.Contains("Mobile Search Deck", html);
        Assert.Contains("Featured Price Changes", html);
    }

    [Fact]
    public async Task UnauthorizedFavorites_RedirectsToLogin()
    {
        await using TestAppFactory factory = new();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        HttpResponseMessage response = await client.GetAsync("/Favorites/Index");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Login_FormSubmission_Works()
    {
        await using TestAppFactory factory = new();
        await factory.SeedUserAsync("webtester", "webtester@example.com", "Password123!");

        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        string loginPage = await client.GetStringAsync("/Account/Login");
        string loginToken = ExtractAntiForgeryToken(loginPage);

        Dictionary<string, string?> loginForm = new()
        {
            ["UserNameOrEmail"] = "webtester",
            ["Password"] = "Password123!",
            ["ReturnUrl"] = "",
            ["__RequestVerificationToken"] = loginToken
        };

        HttpResponseMessage loginResponse = await client.PostAsync(
            "/Account/Login",
            new FormUrlEncodedContent(loginForm!));

        Assert.True(
            loginResponse.StatusCode is HttpStatusCode.Redirect or HttpStatusCode.OK,
            $"Unexpected status code from login: {loginResponse.StatusCode}");

        if (loginResponse.StatusCode == HttpStatusCode.Redirect)
        {
            Assert.Equal("/Account/Profile", loginResponse.Headers.Location?.ToString());
        }

    }

    [Fact]
    public async Task Register_WithInvalidModel_StaysOnForm()
    {
        await using TestAppFactory factory = new();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        string registerPage = await client.GetStringAsync("/Account/Register");
        string registerToken = ExtractAntiForgeryToken(registerPage);

        Dictionary<string, string?> invalidForm = new()
        {
            ["UserName"] = "",
            ["Email"] = "not-an-email",
            ["Password"] = "123",
            ["ConfirmPassword"] = "456",
            ["__RequestVerificationToken"] = registerToken
        };

        HttpResponseMessage response = await client.PostAsync(
            "/Account/Register",
            new FormUrlEncodedContent(invalidForm!));

        string html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Create Account", html);
    }

    private static string ExtractAntiForgeryToken(string html)
    {
        Match match = Regex.Match(
            html,
            "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new InvalidOperationException("Unable to find anti-forgery token.");
        }

        return match.Groups[1].Value;
    }

    private sealed class TestAppFactory : WebApplicationFactory<Program>, IAsyncDisposable
    {
        private readonly string _databasePath =
            System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"takedowntcg-web-tests-{Guid.NewGuid():N}.db");

        public async Task SeedUserAsync(string userName, string email, string password)
        {
            IUserRepository userRepository = new UserRepository(_databasePath);
            IAccountService accountService = new AccountService(userRepository);
            await accountService.CreateAccountAsync(userName, email, password);
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("TAKEDOWNTCG_DB_PATH", _databasePath);

            builder.ConfigureAppConfiguration((_, config) =>
            {
                Dictionary<string, string?> overrides = new()
                {
                    ["Persistence:DatabasePath"] = _databasePath,
                    ["JustTcgApi:ApiKey"] = "",
                    ["JustTcgApi:BaseUrl"] = "https://api.justtcg.com/v1",
                    ["JustTcgApi:ApiKeyHeaderName"] = "x-api-key"
                };

                config.AddInMemoryCollection(overrides);
            });
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            Environment.SetEnvironmentVariable("TAKEDOWNTCG_DB_PATH", null);

            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }

            await DisposeAsyncCore();
        }

        private Task DisposeAsyncCore()
        {
            Dispose();
            return Task.CompletedTask;
        }
    }
}
