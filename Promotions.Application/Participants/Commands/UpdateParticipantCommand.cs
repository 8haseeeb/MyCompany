using MediatR;

namespace Promotions.Application.Participants.Commands
{
    public class UpdateParticipantCommand : IRequest<Unit>
    {
        public int IdAction { get; set; }
        public string CodParticipant { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        public UpdateParticipantCommand(int idAction, string codParticipant, bool flgInclusion)
        {
            IdAction = idAction;
            CodParticipant = codParticipant;
            FlgInclusion = flgInclusion;
        }
    }
}
