using NSubstitute;
using Promotions.Application.PromoActions.Commands;
using Promotions.Application.PromoActions.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.PromoActions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.PromoActions.Handler
{
    public class CreatePromoActionCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreatePromoActionCommandHandler _handler;

        public CreatePromoActionCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreatePromoActionCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_CreatePromoAction()
        {
            // --- ARRANGE ---
            var command = new CreatePromoActionCommand(
                IdAction: 1,
                Name: "Summer Sale",
                CodDiv: "D1",
                DteStartSellIn: DateTime.Now,
                DteEndSellIn: DateTime.Now.AddDays(10),
                DteStartSellOut: DateTime.Now.AddDays(5),
                DteEndSellOut: DateTime.Now.AddDays(15),
                DocumentKey: "DOC123",
                DteToShost: null,
                LevParticipants: 1
            );

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.PromoActions.Received(1).AddAsync(Arg.Any<PromoAction>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
