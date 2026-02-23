using System;

namespace Promotions.Domain.Participants
{
    public class PromoParticipants
    {
        public int IdAction { get; private set; }
        public string CodParticipant { get; private set; } = null!;
        public bool FlgInclusion { get; private set; }

        // Foreign Keys for CustomerRelation
        public string? CodHier { get; private set; }
        public string? CodDiv { get; private set; }
        public string? CodNode { get; private set; }
        public int IdLevel { get; private set; }
        public DateTime DteStart { get; private set; }

        public void Include() => FlgInclusion = true;
        public void Exclude() => FlgInclusion = false;

        // Navigation Properties
        public virtual PromoActions.PromoAction Action { get; private set; } = null!;
        public virtual CustomerRelations.CustomerRelation Relation { get; private set; } = null!;

        private PromoParticipants() { }

        public PromoParticipants(int idAction, string codParticipant, bool flgInclusion, string? codHier, string? codDiv, string? codNode, int idLevel, DateTime dteStart)
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