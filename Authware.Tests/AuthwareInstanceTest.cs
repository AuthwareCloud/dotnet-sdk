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
    public void InitializeApplication_ThrowAuthwareException()
    {
        var app = new AuthwareApplication();
        
        Assert.ThrowsAsync<AuthwareException>(async () => await app.InitializeApplicationAsync(Guid.Empty.ToString()));
    }
}