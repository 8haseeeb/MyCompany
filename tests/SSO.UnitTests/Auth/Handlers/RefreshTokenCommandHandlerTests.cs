using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SSO.Application.Auth.Commands;
using SSO.Application.Auth.Handlers;
using SSO.Application.Interfaces;
using SSO.Domain.RefreshTokens;
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
            _handler = new RefreshTokenCommandHandler(_context, _jwtTokenService);
        }

        [Fact]
        public async Task Handle_Should_ReturnNewTokens_WhenRefreshTokenIsValid()
        {
            // --- ARRANGE ---
            var user = new User("testuser", "test@example.com", "hash");
            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            var oldToken = new RefreshToken
            {
                Token = "old_refresh_token",
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };
            _context.RefreshTokens.Add(oldToken);
            await _context.SaveChangesAsync(CancellationToken.None);

            _jwtTokenService.GenerateToken(Arg.Any<User>(), Arg.Any<string>()).Returns("new_access_token");

            var command = new RefreshTokenCommand("old_refresh_token");

            // --- ACT ---
            var result = await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.Equal("new_access_token", result.AccessToken);
            Assert.NotEqual("old_refresh_token", result.RefreshToken);
            
            var updatedOldToken = await _context.RefreshTokens.FirstAsync(rt => rt.Token == "old_refresh_token");
            Assert.True(updatedOldToken.IsRevoked);
            
            var newTokenInDb = await _context.RefreshTokens.AnyAsync(rt => rt.Token == result.RefreshToken);
            Assert.True(newTokenInDb);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenTokenIsExpired()
        {
            // --- ARRANGE ---
            var user = new User("testuser", "test@example.com", "hash");
            _context.Users.Add(user);
            
            var expiredToken = new RefreshToken
            {
                Token = "expired_token",
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                IsRevoked = false
            };
            _context.RefreshTokens.Add(expiredToken);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new RefreshTokenCommand("expired_token");

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenTokenIsRevoked()
        {
            // --- ARRANGE ---
            var user = new User("testuser", "test@example.com", "hash");
            _context.Users.Add(user);
            
            var revokedToken = new RefreshToken
            {
                Token = "revoked_token",
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = true
            };
            _context.RefreshTokens.Add(revokedToken);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new RefreshTokenCommand("revoked_token");

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
