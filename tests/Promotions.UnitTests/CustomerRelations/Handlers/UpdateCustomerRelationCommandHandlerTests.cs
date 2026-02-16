using NSubstitute;
using Promotions.Application.CustomerRelations.Commands;
using Promotions.Application.CustomerRelations.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.CustomerRelations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.CustomerRelations.Handlers
{
    public class UpdateCustomerRelationCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateCustomerRelationCommandHandler _handler;

        public UpdateCustomerRelationCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdateCustomerRelationCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_UpdateCustomerRelation_WhenExists()
        {
            // --- ARRANGE ---
            var dteStart = DateTime.Now;
            var command = new UpdateCustomerRelationCommand("H1", "D1", "N1", 2, dteStart, "NEW_PARENT", null);
            var existingRelation = new CustomerRelation("H1", "D1", "N1", 2, dteStart, "OLD_PARENT");
            
            _unitOfWork.CustomerRelations.GetByIdAsync("H1", "D1", "N1", 2, dteStart).Returns(existingRelation);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.Equal("NEW_PARENT", existingRelation.CodParentNode);
            await _unitOfWork.CustomerRelations.Received(1).UpdateAsync(existingRelation);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var dteStart = DateTime.Now;
            var command = new UpdateCustomerRelationCommand("H1", "D1", "N1", 2, dteStart, "ANY", null);
            _unitOfWork.CustomerRelations.GetByIdAsync("H1", "D1", "N1", 2, dteStart).Returns((CustomerRelation)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
