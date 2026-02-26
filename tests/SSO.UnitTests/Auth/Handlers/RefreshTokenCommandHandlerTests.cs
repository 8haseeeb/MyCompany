using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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
    public class RefreshTokenCommandHandlerTests : IDisposable
    {
        private readonly IdentityDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IdentityDbContext(options);
            _jwtTokenService = Substitute.For<IJwtTokenService>();
            _handler = new RefreshTokenCommandHandler(_context, _jwtTokenService, NullLogger<RefreshTokenCommandHandler>.Instance);
        }

        [Fact]
        public async Task Handle_Should_ReturnNewTokens_WhenRefreshTokenIsValid()
        {
            // --- ARRANGE ---
            var oldRefreshToken = "old_refresh_token";
            var user = new User("testuser", "test@example.com", "hash");
            user.UpdateRefreshToken(oldRefreshToken, DateTime.UtcNow.AddDays(1));
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            _jwtTokenService.GenerateToken(Arg.Any<User>(), Arg.Any<string>()).Returns("new_access_token");

            var command = new RefreshTokenCommand(oldRefreshToken);

            // --- ACT ---
            var result = await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.Equal("new_access_token", result.AccessToken);
            Assert.NotEqual(oldRefreshToken, result.RefreshToken);
            
            var userInDb = await _context.Users.FirstAsync(u => u.Id == user.Id);
            Assert.Equal(result.RefreshToken, userInDb.RefreshToken);
            Assert.True(userInDb.RefreshTokenExpiry > DateTime.UtcNow);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenTokenIsExpired()
        {
            // --- ARRANGE ---
            var expiredToken = "expired_token";
            var user = new User("testuser", "test@example.com", "hash");
            user.UpdateRefreshToken(expiredToken, DateTime.UtcNow.AddDays(-1));
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new RefreshTokenCommand(expiredToken);

            // --- ACT & ASSERT ---
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Invalid or expired refresh token", ex.Message);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenTokenNotFound()
        {
            // --- ARRANGE ---
            var command = new RefreshTokenCommand("non_existent_token");

            // --- ACT & ASSERT ---
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Invalid or expired refresh token", ex.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
