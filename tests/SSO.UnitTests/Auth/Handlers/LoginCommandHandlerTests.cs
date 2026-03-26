using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SSO.Application.Auth;
using SSO.Application.Auth.Commands;
using SSO.Application.Auth.Handlers;
using SSO.Application.Interfaces;
using SSO.Domain.Users;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            _jwtTokenService.GenerateToken(Arg.Any<User>(), Arg.Any<string>()).Returns("access_token");
            _passwordHasher = Substitute.For<IPasswordHasher>();
            _passwordHasher.LooksLikeBcryptHash(Arg.Any<string>()).Returns(true);

            var sessionTokens = new UserSessionTokenService(_context, _jwtTokenService);
            _handler = new LoginCommandHandler(_userRepository, sessionTokens, _passwordHasher);
        }

        [Fact]
        public async Task Handle_Should_ReturnTokens_WhenCredentialsAreValid()
        {
            var email = "test@example.com";
            var password = "password123";
            var user = new User("testuser", email, "hashed_password");

            _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify("hashed_password", password).Returns(true);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new LoginCommand(email, password);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("access_token", result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("testuser", result.UserName);

            var userInDb = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.NotNull(userInDb.RefreshToken);
            Assert.True(userInDb.RefreshTokenExpiry > DateTime.UtcNow);
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidCredentials_WhenUserNotFound()
        {
            _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);
            var command = new LoginCommand("wrong@example.com", "password");

            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_ThrowInvalidCredentials_WhenPasswordIsIncorrect()
        {
            var user = new User("testuser", "test@example.com", "hashed_password");
            _userRepository.GetByEmailAsync(user.Email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify("hashed_password", "wrong_password").Returns(false);

            var command = new LoginCommand(user.Email, "wrong_password");

            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _handler.Handle(command, CancellationToken.None));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
