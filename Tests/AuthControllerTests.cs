using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using SportsAIPredictionAPI.Controllers;
using SportsAIPredictionAPI.Data;
using SportsAIPredictionAPI.DTOs;
using SportsAIPredictionAPI.Models;
using Xunit;

namespace SportsAIPredictionAPI.Tests;

public class AuthControllerTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IConfiguration GetConfig()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "supersecretkey1234567890abcdefgh" }
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    [Fact]
    public async Task Register_ShouldReturn200_WhenValidRequest()
    {
        var context = GetInMemoryContext();
        var config = GetConfig();
        var controller = new AuthController(context, config);

        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@test.com",
            Password = "Test1234!"
        };

        var result = await controller.Register(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenEmailAlreadyExists()
    {
        var context = GetInMemoryContext();
        var config = GetConfig();
        var controller = new AuthController(context, config);

        context.Users.Add(new User
        {
            Username = "existing",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
        });
        await context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Username = "newuser",
            Email = "test@test.com",
            Password = "Test1234!"
        };

        var result = await controller.Register(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenValidCredentials()
    {
        var context = GetInMemoryContext();
        var config = GetConfig();
        var controller = new AuthController(context, config);

        context.Users.Add(new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test1234!")
        });
        await context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test1234!"
        };

        var result = await controller.Login(request);
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(ok.Value);

        Assert.NotEmpty(response.Token);
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenInvalidCredentials()
    {
        var context = GetInMemoryContext();
        var config = GetConfig();
        var controller = new AuthController(context, config);

        var request = new LoginRequest
        {
            Email = "wrong@test.com",
            Password = "wrongpassword"
        };

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
