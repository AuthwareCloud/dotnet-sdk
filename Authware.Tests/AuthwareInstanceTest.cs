using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authware.Exceptions;
using NUnit.Framework;

namespace Authware.Tests;

public class AuthwareInstanceTest
{
    // These credentials will not work in production, these are specific to your dev environment
    private const string ApplicationId = "baf3d091-3626-40a0-afb9-2c2eda2c6e45";
    private const string Username = "Test";
    private const string Password = "UhokYBGs3yxWzPROdsJx";
    private const string ApiKey = "745ceb61-fea1-46f6-89d8-05a0f96270ed57f4c0a2-4d2b-4be4-92e3-218b3e9b42c8";
    private const string Email = "test@example.com";
    private const string Token = "ad00a70a-49ee-44cc-9b31-4d3e38823236";

    [Test]
    public async Task InitializeApplication_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(ApplicationId);
        Assert.IsNotNull(applicationInfo);
    }

    [Test]
    public async Task Register_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(ApplicationId);
        Assert.IsNotNull(applicationInfo);

        var response =
            await app.RegisterAsync(Username, Password, Email, Token);
        Assert.IsTrue(response.Success);
    }
    
    [Test]
    public async Task CheckKeyRegen_NoException()
    {
        // Init the app
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(ApplicationId);
        Assert.IsNotNull(applicationInfo);

        // Log the user in
        var profile = await app.LoginAsync(Username, Password);
        Assert.IsNotNull(profile?.Username);

        // Get and store the previous API key
        var previousKey = profile.ApiKey;

        // Regenerate the key
        var response = await app.RegenerateApiKeyAsync(Password);
        
        // Ensure that it responded with true
        Assert.IsTrue(response.Success);

        // Check that the key changed by getting the new profile and comparing the two keys
        var newProfile = await app.GetUserProfileAsync();
        Assert.IsTrue(previousKey != newProfile.ApiKey);
    }

    [Test]
    public async Task CheckApiExec_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(ApplicationId);
        Assert.IsNotNull(applicationInfo);

        var profile = await app.LoginAsync(Username, Password);
        Assert.IsNotNull(profile?.Username);

        var response = await app.ExecuteApiAsync(app.ApplicationInformation.Apis.FirstOrDefault().Id.ToString(),
            new Dictionary<string, object>
            {
                {"test", "test1234567"}
            });
        
        Assert.IsTrue(response.Success);
    }

    [Test]
    public async Task CheckLogin_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(ApplicationId);
        Assert.IsNotNull(applicationInfo);

        var profile = await app.LoginAsync(Username, Password);
        Assert.IsNotNull(profile?.Username);
    }
    
    [Test]
    public async Task CheckApiKeyAuth_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(ApplicationId);
        Assert.IsNotNull(applicationInfo);

        // Authorize here with the predefined API key
        app.AuthorizeWithApiKey(ApiKey);
        
        // Try and get the users profile
        var profile = await app.GetUserProfileAsync();
        Assert.IsNotNull(profile?.Username);
    }
    
    
    [Test]
    public void InitializeApplication_ThrowAuthwareException()
    {
        var app = new AuthwareApplication();
        
        Assert.ThrowsAsync<AuthwareException>(async () => await app.InitializeApplicationAsync(Guid.Empty.ToString()));
    }
}