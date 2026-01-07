using System;

namespace Promotions.Domain.PromoActions
{
    public class PromoAction
    {
        
        public int IdAction { get; set; }        // ID_ACTION

        public string Name { get; set; } = null!; // DESACTION

        public string CodDiv { get; set; } = null!;
        public string CodContractor { get; set; } = null!; // Maps to CodNode
        
        public DateTime DteStartSellIn { get; set; }
        public DateTime DteEndSellIn { get; set; }

        public DateTime DteStartSellOut { get; set; }
        public DateTime DteEndSellOut { get; set; }

        public string? DocumentKey { get; set; }
        public DateTime? DteToShost { get; set; }

        public int? LevParticipants { get; set; }

        // Navigation Properties
        // Navigation Properties
        public virtual ICollection<Domain.Products.PromoProduct> Products { get; set; } = new List<Domain.Products.PromoProduct>();
        public virtual ICollection<Domain.Participants.PromoParticipants> Participants { get; set; } = new List<Domain.Participants.PromoParticipants>();
        public virtual ICollection<Domain.DeliveryPoints.PromoDeliveryPoint> DeliveryPoints { get; set; } = new List<Domain.DeliveryPoints.PromoDeliveryPoint>();
    }
}
