using System;

namespace Promotions.Domain.Participants
{
    public class PromoParticipants
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

        public void Include() => FlgInclusion = true;
        public void Exclude() => FlgInclusion = false;

        // Navigation Properties
        public virtual PromoActions.PromoAction Action { get; set; } = null!;
        public virtual CustomerRelations.CustomerRelation Relation { get; set; } = null!;
    }
}