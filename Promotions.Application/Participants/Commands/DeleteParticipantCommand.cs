using MediatR;

namespace Promotions.Application.Participants.Commands
{
    public class DeleteParticipantCommand : IRequest
    {
        public int IdAction { get; set; }
        public string CodParticipant { get; set; } = null!;

        public DeleteParticipantCommand(int idAction, string codParticipant)
        {
            IdAction = idAction;
            CodParticipant = codParticipant;
        }
    }
}
