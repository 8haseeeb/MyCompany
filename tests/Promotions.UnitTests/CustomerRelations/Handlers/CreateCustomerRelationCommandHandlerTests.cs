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
    public class CreateCustomerRelationCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateCustomerRelationCommandHandler _handler;

        public CreateCustomerRelationCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateCustomerRelationCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_CreateCustomerRelation()
        {
            // --- ARRANGE ---
            var command = new CreateCustomerRelationCommand(
                codHier: "H1", codDiv: "D1", codNode: "N1", idLevel: 2, dteStart: DateTime.Now, codParentNode: "ROOT", dteEnd: null
            );

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.CustomerRelations.Received(1).AddAsync(Arg.Any<CustomerRelation>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
