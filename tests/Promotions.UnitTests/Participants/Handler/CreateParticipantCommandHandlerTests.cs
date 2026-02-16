using NSubstitute;
using Promotions.Application.Participants.Commands;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Participants;
using Promotions.Domain.CustomerRelations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.Participants.Handler
{
    public class CreateParticipantCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateParticipantCommandHandler _handler;

        public CreateParticipantCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateParticipantCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_Create_Participant_And_CustomerRelation_When_Not_Exists()
        {
            // --- ARRANGE ---
            var command = new CreateParticipantCommand(
                idAction: 1,
                codParticipant: "PART1",
                flgInclusion: true,
                codHier: "H1",
                codDiv: "D1",
                codNode: "N1",
                idLevel: 2,
                dteStart: DateTime.Now
            );

            // Mock ExistsAsync to return false
            _unitOfWork.CustomerRelations.ExistsAsync(
                command.CodHier,
                command.CodDiv,
                command.CodNode,
                command.IdLevel,
                command.DteStart).Returns(false);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            // Verify CustomerRelation was added
            await _unitOfWork.CustomerRelations.Received(1).AddAsync(Arg.Any<CustomerRelation>());
            
            // Verify Participant was added
            await _unitOfWork.Participants.Received(1).AddAsync(Arg.Any<PromoParticipants>());

            // Verify SaveChanges was called
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_Only_Create_Participant_When_CustomerRelation_Exists()
        {
            // --- ARRANGE ---
            var command = new CreateParticipantCommand(
                1, "PART1", true, "H1", "D1", "N1", 2, DateTime.Now
            );

            // Mock ExistsAsync to return true
            _unitOfWork.CustomerRelations.ExistsAsync(
                command.CodHier,
                command.CodDiv,
                command.CodNode,
                command.IdLevel,
                command.DteStart).Returns(true);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            // Verify CustomerRelation was NOT added
            await _unitOfWork.CustomerRelations.DidNotReceive().AddAsync(Arg.Any<CustomerRelation>());
            
            // Verify Participant was still added
            await _unitOfWork.Participants.Received(1).AddAsync(Arg.Any<PromoParticipants>());

            // Verify SaveChanges was called
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
