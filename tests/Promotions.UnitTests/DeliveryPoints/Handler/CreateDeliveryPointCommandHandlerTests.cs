using NSubstitute;
using Promotions.Application.DeliveryPoints.Commands;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.DeliveryPoints;
using Promotions.Domain.CustomerRelations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.DeliveryPoints.Handler
{
    public class CreateDeliveryPointCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateDeliveryPointCommandHandler _handler;

        public CreateDeliveryPointCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateDeliveryPointCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_Create_DeliveryPoint_And_CustomerRelation_When_Not_Exists()
        {
            // --- ARRANGE ---
            var command = new CreateDeliveryPointCommand(
                idAction: 1, codDeliveryPoint: "DP1", flgInclusion: true,
                codHier: "H1", codDiv: "D1", codNode: "N1", idLevel: 2, dteStart: DateTime.Now
            );

            _unitOfWork.CustomerRelations.ExistsAsync(
                command.CodHier, command.CodDiv, command.CodNode, command.IdLevel, command.DteStart
            ).Returns(false);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.CustomerRelations.Received(1).AddAsync(Arg.Any<CustomerRelation>());
            await _unitOfWork.DeliveryPoints.Received(1).AddAsync(Arg.Any<PromoDeliveryPoint>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
