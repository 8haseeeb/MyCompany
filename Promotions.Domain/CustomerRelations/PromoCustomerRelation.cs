using System;

namespace Promotions.Domain.CustomerRelations
{
    public class CustomerRelation
    {
        public string CodHier { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public int IdLevel { get; set; }
        public DateTime DteStart { get; set; }

        public string CodParentNode { get; set; } = null!;
        public DateTime? DteEnd { get; set; }

        // Navigation Properties
        public virtual ICollection<Domain.Participants.PromoParticipants> Participants { get; set; } = new List<Domain.Participants.PromoParticipants>();
        public virtual ICollection<Domain.DeliveryPoints.PromoDeliveryPoint> DeliveryPoints { get; set; } = new List<Domain.DeliveryPoints.PromoDeliveryPoint>();
    }
}
