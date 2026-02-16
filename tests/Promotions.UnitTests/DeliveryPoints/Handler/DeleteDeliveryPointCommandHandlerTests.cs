using NSubstitute;
using Promotions.Application.DeliveryPoints.Commands;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.DeliveryPoints;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.DeliveryPoints.Handler
{
    public class DeleteDeliveryPointCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteDeliveryPointCommandHandler _handler;

        public DeleteDeliveryPointCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeleteDeliveryPointCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_Delete_DeliveryPoint_WhenExists()
        {
            // --- ARRANGE ---
            var command = new DeleteDeliveryPointCommand(1, "DP1");
            var existingDP = new PromoDeliveryPoint(1, "DP1", true, "H1", "D1", "N1", 2, DateTime.Now);
            _unitOfWork.DeliveryPoints.GetByIdAsync(1, "DP1").Returns(existingDP);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.DeliveryPoints.Received(1).DeleteAsync(existingDP);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new DeleteDeliveryPointCommand(1, "NOTFOUND");
            _unitOfWork.DeliveryPoints.GetByIdAsync(1, "NOTFOUND").Returns((PromoDeliveryPoint)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
