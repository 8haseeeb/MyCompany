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
    public class UpdatePromoActionCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdatePromoActionCommandHandler _handler;

        public UpdatePromoActionCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdatePromoActionCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_UpdatePromoAction_WhenExists()
        {
            // --- ARRANGE ---
            var command = new UpdatePromoActionCommand(
                IdAction: 1,
                Name: "Updated Name",
                DteStartSellIn: null,
                DteEndSellIn: null,
                DteStartSellOut: null,
                DteEndSellOut: null,
                DocumentKey: "NEW_DOC",
                DteToShost: null,
                LevParticipants: 2
            );

            var existingAction = new PromoAction(1, "Old Name", "D1");
            existingAction.UpdateSellInDates(DateTime.Now, DateTime.Now.AddDays(1));
            existingAction.UpdateSellOutDates(DateTime.Now, DateTime.Now.AddDays(1));
            
            _unitOfWork.PromoActions.GetByIdAsync(1).Returns(existingAction);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.Equal("Updated Name", existingAction.Name);
            Assert.Equal("NEW_DOC", existingAction.DocumentKey);
            Assert.Equal(2, existingAction.LevParticipants);
            
            await _unitOfWork.PromoActions.Received(1).UpdateAsync(existingAction);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new UpdatePromoActionCommand(1, "Any", null, null, null, null, null, null, null);
            _unitOfWork.PromoActions.GetByIdAsync(1).Returns((PromoAction)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
