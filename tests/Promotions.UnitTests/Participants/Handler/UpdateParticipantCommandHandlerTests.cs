using NSubstitute;
using Promotions.Application.Participants.Commands;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Participants;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.Participants.Handler
{
    public class UpdateParticipantCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateParticipantCommandHandler _handler;

        public UpdateParticipantCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdateParticipantCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_Update_Participant_Status_When_Exists()
        {
            // --- ARRANGE ---
            var command = new UpdateParticipantCommand(1, "PART1", true); // Include
            
            var existingParticipant = new PromoParticipants(1, "PART1", false, "H1", "D1", "N1", 2, DateTime.Now);
            
            _unitOfWork.Participants.GetByIdAsync(1, "PART1").Returns(existingParticipant);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.True(existingParticipant.FlgInclusion);
            await _unitOfWork.Participants.Received(1).UpdateAsync(existingParticipant);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Not_Exists()
        {
            // --- ARRANGE ---
            var command = new UpdateParticipantCommand(1, "NON_EXISTENT", true);
            _unitOfWork.Participants.GetByIdAsync(1, "NON_EXISTENT").Returns((PromoParticipants)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
