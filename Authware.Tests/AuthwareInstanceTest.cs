using System;
using System.Threading.Tasks;
using Authware.Exceptions;
using NUnit.Framework;

namespace Authware.Tests;

public class AuthwareInstanceTest
{
    private string _applicationGuid;
    [SetUp]
    public void Setup()
    {
        _applicationGuid = "e3d3844a-d0e3-4fc4-90d4-b606c12a2466";
    }

    [Test]
    public async Task InitializeApplication_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(_applicationGuid);
        Assert.IsNotNull(applicationInfo);
    }

    [Test]
    public async Task Register_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(_applicationGuid);
        Assert.IsNotNull(applicationInfo);

        var response =
            await app.RegisterAsync("Test", "Test.Password.Do.Not.Use.This1", "test@example.com", "ad00a70a-49ee-44cc-9b31-4d3e38823236");
        Assert.IsTrue(response.Success);
    }

    [Test]
    public async Task CheckLogin_NoException()
    {
        var app = new AuthwareApplication();
        var applicationInfo = await app.InitializeApplicationAsync(_applicationGuid);
        Assert.IsNotNull(applicationInfo);

        var profile = await app.LoginAsync("Test", "Test.Password.Do.Not.Use.This1");
        Assert.IsNotNull(profile?.Username);
    }
    
    
    [Test]
    public void InitializeApplication_ThrowAuthwareException()
    {
        var app = new AuthwareApplication();
        
        Assert.ThrowsAsync<AuthwareException>(async () => await app.InitializeApplicationAsync(Guid.Empty.ToString()));
    }
}