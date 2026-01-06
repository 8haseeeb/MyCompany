using MediatR;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class CreateDeliveryPointCommand : IRequest<Unit>
    {
        public int IdAction { get; }
        public string CodDeliveryPoint { get; }
        public bool FlgInclusion { get; }

        // Foreign Keys for CustomerRelation
        public string CodHier { get; }
        public string CodDiv { get; }
        public string CodNode { get; }
        public int IdLevel { get; }
        public DateTime DteStart { get; }

        public CreateDeliveryPointCommand(
            int idAction,
            string codDeliveryPoint,
            bool flgInclusion,
            string codHier,
            string codDiv,
            string codNode,
            int idLevel,
            DateTime dteStart)
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
    }
}
