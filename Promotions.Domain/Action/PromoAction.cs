using System;
using System.Collections.Generic;
using Promotions.Domain.Products;
using Promotions.Domain.Participants;
using Promotions.Domain.DeliveryPoints;

namespace Promotions.Domain.PromoActions
{
    public class PromoAction
    {
        public int IdAction { get; private set; }
        public string Name { get; private set; } = null!;
        public string? CodDiv { get; private set; }
        
        public DateTime DteStartSellIn { get; private set; }
        public DateTime DteEndSellIn { get; private set; }

        public DateTime DteStartSellOut { get; private set; }
        public DateTime DteEndSellOut { get; private set; }

        public string? DocumentKey { get; private set; }
        public DateTime? DteToShost { get; private set; }

        public int? LevParticipants { get; private set; }

        // Navigation Properties
        public virtual ICollection<PromoProduct> Products { get; private set; } = new List<PromoProduct>();
        public virtual ICollection<PromoParticipants> Participants { get; private set; } = new List<PromoParticipants>();
        public virtual ICollection<PromoDeliveryPoint> DeliveryPoints { get; private set; } = new List<PromoDeliveryPoint>();

        private PromoAction() { }

        public PromoAction(int idAction, string name, string? codDiv)
        {
            if (idAction <= 0) throw new ArgumentException("IdAction must be positive.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(codDiv)) throw new ArgumentException("CodDiv is required.");

            IdAction = idAction;
            Name = name;
            CodDiv = codDiv;
        }

        public void UpdateBasicInfo(string name, string? codDiv, string? documentKey, int? levParticipants)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            Name = name;
            CodDiv = codDiv;
            DocumentKey = documentKey;
            LevParticipants = levParticipants;
        }

        public void UpdateSellInDates(DateTime start, DateTime end)
        {
            if (start >= end) throw new ArgumentException("Start Sell In date must be before End Sell In date.");
            DteStartSellIn = start;
            DteEndSellIn = end;
        }

        public void UpdateSellOutDates(DateTime start, DateTime end)
        {
            if (start >= end) throw new ArgumentException("Start Sell Out date must be before End Sell Out date.");
            DteStartSellOut = start;
            DteEndSellOut = end;
        }

        public void SetHostDate(DateTime? hostDate)
        {
            DteToShost = hostDate;
        }
    }
}
