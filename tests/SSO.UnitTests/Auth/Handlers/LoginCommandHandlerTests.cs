using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SSO.Application.Auth.Commands;
using SSO.Application.Auth.Handlers;
using SSO.Application.Interfaces;
using SSO.Domain.Users;
using SSO.Infrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.UnitTests.Auth.Handlers
{
    public class LoginCommandHandlerTests : IDisposable
    {
        private readonly IdentityDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IdentityDbContext(options);
            _userRepository = Substitute.For<IUserRepository>();
            _jwtTokenService = Substitute.For<IJwtTokenService>();
            _passwordHasher = Substitute.For<IPasswordHasher>();
            
            _handler = new LoginCommandHandler(_userRepository, _jwtTokenService, _passwordHasher, _context);
        }

        [Fact]
        public async Task Handle_Should_ReturnTokens_WhenCredentialsAreValid()
        {
            // --- ARRANGE ---
            var email = "test@example.com";
            var password = "password123";
            var user = new User("testuser", email, "hashed_password");
            
            _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify("hashed_password", password).Returns(true);
            _jwtTokenService.GenerateToken(user, Arg.Any<string>()).Returns("access_token");

            var command = new LoginCommand(email, password);

            // --- ACT ---
            var result = await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.Equal("access_token", result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("testuser", result.UserName);
            
            var userInDb = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.NotNull(userInDb.RefreshToken);
            Assert.True(userInDb.RefreshTokenExpiry > DateTime.UtcNow);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenUserNotFound()
        {
            // --- ARRANGE ---
            _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);
            var command = new LoginCommand("wrong@example.com", "password");

            // --- ACT & ASSERT ---
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Invalid credentials", ex.Message);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenPasswordIsIncorrect()
        {
            // --- ARRANGE ---
            var user = new User("testuser", "test@example.com", "hashed_password");
            _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify("hashed_password", "wrong_password").Returns(false);

            var command = new LoginCommand(user.Email, "wrong_password");

            // --- ACT & ASSERT ---
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Invalid credentials", ex.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
