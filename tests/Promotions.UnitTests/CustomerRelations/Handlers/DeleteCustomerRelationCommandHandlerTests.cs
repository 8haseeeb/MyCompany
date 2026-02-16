using NSubstitute;
using Promotions.Application.CustomerRelations.Commands;
using Promotions.Application.CustomerRelations.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.CustomerRelations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.CustomerRelations.Handlers
{
    public class DeleteCustomerRelationCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteCustomerRelationCommandHandler _handler;

        public DeleteCustomerRelationCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeleteCustomerRelationCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_DeleteCustomerRelation_WhenExists()
        {
            // --- ARRANGE ---
            var dteStart = DateTime.Now;
            var command = new DeleteCustomerRelationCommand("H1", "D1", "N1", 2, dteStart);
            var existingRelation = new CustomerRelation("H1", "D1", "N1", 2, dteStart, "ROOT");
            
            _unitOfWork.CustomerRelations.GetByIdAsync("H1", "D1", "N1", 2, dteStart).Returns(existingRelation);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.CustomerRelations.Received(1).DeleteAsync(existingRelation);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var dteStart = DateTime.Now;
            var command = new DeleteCustomerRelationCommand("H1", "D1", "N1", 2, dteStart);
            _unitOfWork.CustomerRelations.GetByIdAsync("H1", "D1", "N1", 2, dteStart).Returns((CustomerRelation)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
