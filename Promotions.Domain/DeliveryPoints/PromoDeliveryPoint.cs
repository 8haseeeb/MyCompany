using System;

namespace Promotions.Domain.DeliveryPoints
{
    public class PromoDeliveryPoint
    {
        public int IdAction { get; private set; }
        public string CodDeliveryPoint { get; private set; } = null!;
        public bool FlgInclusion { get; private set; }

        // Foreign Keys for CustomerRelation
        public string CodHier { get; private set; } = null!;
        public string CodDiv { get; private set; } = null!;
        public string CodNode { get; private set; } = null!;
        public int IdLevel { get; private set; }
        public DateTime DteStart { get; private set; }

        // Navigation Properties
        public virtual PromoActions.PromoAction Action { get; private set; } = null!;
        public virtual CustomerRelations.CustomerRelation Relation { get; private set; } = null!;

        private PromoDeliveryPoint() { }

        public PromoDeliveryPoint(int idAction, string codDeliveryPoint, bool flgInclusion, string codHier, string codDiv, string codNode, int idLevel, DateTime dteStart)
        {
            IdAction = idAction;
            CodDeliveryPoint = codDeliveryPoint;
            FlgInclusion = flgInclusion;
            CodHier = codHier;
            CodDiv = codDiv;
            CodNode = codNode;
            IdLevel = idLevel;
            DteStart = dteStart;
        }

        public void Include() => FlgInclusion = true;
        public void Exclude() => FlgInclusion = false;
    }
}
