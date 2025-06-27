using Xunit;
using BookingSystem.Services;
using BookingSystem.Models;
using BookingSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BookingSystem.Configurations;

namespace BookingSystem.Tests
{
    public class UserServiceTest
    {
        private UserService GetUserService(DbContextOptions<BookingDbContext> options)
        {
            var context = new BookingDbContext(options);
            return new UserService(context);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "RegisterUserDb").Options;
            var service = GetUserService(options);
            var request = new RegisterRequest { UserName = "testuser", Email = "test@example.com", Password = "Password123" };
            var result = await service.RegisterAsync(request);
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "LoginUserDb").Options;
            var service = GetUserService(options);
            var registerRequest = new RegisterRequest { UserName = "testuser", Email = "test@example.com", Password = "Password123" };
            await service.RegisterAsync(registerRequest);
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "Password123" };
            var user = await service.AuthenticateAsync(loginRequest);
            Assert.NotNull(user);
            Assert.Equal("testuser", user.UserName);
        }
    }
}
