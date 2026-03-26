using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SSO.Application.Auth;
using SSO.Application.Auth.Commands;
using SSO.Application.Auth.Handlers;
using SSO.Infrastructure.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.UnitTests.Auth.Handlers
{
    public class RegisterCommandHandlerTests : IDisposable
    {
        private readonly IdentityDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly RegisterCommandHandler _handler;

        public RegisterCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IdentityDbContext(options);
            _passwordHasher = Substitute.For<IPasswordHasher>();
            _handler = new RegisterCommandHandler(_context, _passwordHasher, NullLogger<RegisterCommandHandler>.Instance);
        }

        [Fact]
        public async Task Handle_Should_RegisterUser_WhenNotExists()
        {
            var command = new RegisterCommand("testuser", "Test@Example.com", "password123");
            _passwordHasher.Hash("password123").Returns("hashed_password");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("testuser", result.UserName);
            Assert.Contains("log in", result.Message, StringComparison.OrdinalIgnoreCase);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(user);
            Assert.Equal("testuser", user.UserName);
            Assert.Equal("hashed_password", user.PasswordHash);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public async Task Handle_Should_ThrowDuplicateUserException_WhenUserAlreadyExists()
        {
            var existingUser = new SSO.Domain.Users.User("existing", "test@example.com", "hash");
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync(CancellationToken.None);

            var command = new RegisterCommand("newuser", "test@example.com", "password123");

            await Assert.ThrowsAsync<DuplicateUserException>(() => _handler.Handle(command, CancellationToken.None));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
