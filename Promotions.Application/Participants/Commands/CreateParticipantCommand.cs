using MediatR;

namespace Promotions.Application.Participants.Commands
{
    public class CreateParticipantCommand : IRequest<Unit>
    {
        public int IdAction { get; set; }
        public string CodParticipant { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        // Foreign Keys for CustomerRelation
        public string CodHier { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public int IdLevel { get; set; }
        public DateTime DteStart { get; set; }

        public CreateParticipantCommand(int idAction, string codParticipant, bool flgInclusion,
            string codHier, string codDiv, string codNode, int idLevel, DateTime dteStart)
        {
            IdAction = idAction;
            CodParticipant = codParticipant;
            FlgInclusion = flgInclusion;
            CodHier = codHier;
            CodDiv = codDiv;
            CodNode = codNode;
            IdLevel = idLevel;
            DteStart = dteStart;
        }
    }
}
