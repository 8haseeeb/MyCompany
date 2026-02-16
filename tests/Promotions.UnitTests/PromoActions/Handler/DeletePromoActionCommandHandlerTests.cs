using NSubstitute;
using Promotions.Application.PromoActions.Commands;
using Promotions.Application.PromoActions.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.PromoActions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.PromoActions.Handler
{
    public class DeletePromoActionCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeletePromoActionCommandHandler _handler;

        public DeletePromoActionCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeletePromoActionCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_DeletePromoAction_WhenExists()
        {
            // --- ARRANGE ---
            var command = new DeletePromoActionCommand(1);
            var existingAction = new PromoAction(1, "Summer Sale", "D1");

            _unitOfWork.PromoActions.GetByIdAsync(1).Returns(existingAction);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.PromoActions.Received(1).DeleteAsync(existingAction);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new DeletePromoActionCommand(1);
            _unitOfWork.PromoActions.GetByIdAsync(1).Returns((PromoAction)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
