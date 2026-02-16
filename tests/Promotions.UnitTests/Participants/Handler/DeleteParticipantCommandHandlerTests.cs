using NSubstitute;
using Promotions.Application.Participants.Commands;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Participants;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.Participants.Handler
{
    public class DeleteParticipantCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteParticipantCommandHandler _handler;

        public DeleteParticipantCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeleteParticipantCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_Delete_Participant_When_Exists()
        {
            // --- ARRANGE ---
            var command = new DeleteParticipantCommand(1, "PART1");
            var existingParticipant = new PromoParticipants(1, "PART1", true, "H1", "D1", "N1", 2, DateTime.Now);
            
            _unitOfWork.Participants.GetByIdAsync(1, "PART1").Returns(existingParticipant);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.Participants.Received(1).DeleteAsync(existingParticipant);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Not_Exists()
        {
            // --- ARRANGE ---
            var command = new DeleteParticipantCommand(1, "NON_EXISTENT");
            _unitOfWork.Participants.GetByIdAsync(1, "NON_EXISTENT").Returns((PromoParticipants)null!);

            // --- ACT & ASSERT ---
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("not found", exception.Message);
        }
    }
}
