using System;
using System.Collections.Generic;
using Promotions.Domain.Participants;
using Promotions.Domain.DeliveryPoints;

namespace Promotions.Domain.CustomerRelations
{
    public class CustomerRelation
    {
        public string? CodHier { get; private set; }
        public string? CodDiv { get; private set; }
        public string? CodNode { get; private set; }
        public int IdLevel { get; private set; }
        public DateTime DteStart { get; private set; }

        public string? CodParentNode { get; private set; }
        public DateTime? DteEnd { get; private set; }

        // Navigation Properties
        public virtual ICollection<PromoParticipants> Participants { get; private set; } = new List<PromoParticipants>();
        public virtual ICollection<PromoDeliveryPoint> DeliveryPoints { get; private set; } = new List<PromoDeliveryPoint>();
        
        private CustomerRelation() { }

        public CustomerRelation(string? codHier, string? codDiv, string? codNode, int idLevel, DateTime dteStart, string? codParentNode = null)
        {
            if (string.IsNullOrWhiteSpace(codHier)) throw new ArgumentException("CodHier is required.");
            if (string.IsNullOrWhiteSpace(codDiv)) throw new ArgumentException("CodDiv is required.");
            if (string.IsNullOrWhiteSpace(codNode)) throw new ArgumentException("CodNode is required.");

            CodHier = codHier;
            CodDiv = codDiv;
            CodNode = codNode;
            IdLevel = idLevel;
            DteStart = dteStart;
            CodParentNode = codParentNode;
        }

        public void SetEndDate(DateTime? dteEnd)
        {
            if (dteEnd.HasValue && dteEnd.Value < DteStart)
                throw new ArgumentException("End date cannot be before start date.");
            DteEnd = dteEnd;
        }

        public void UpdateHierarchy(string codParentNode)
        {
            CodParentNode = codParentNode;
        }
    }
}
