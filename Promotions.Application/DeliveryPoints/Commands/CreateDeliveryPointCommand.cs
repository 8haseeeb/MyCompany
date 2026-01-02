using MediatR;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class CreateDeliveryPointCommand : IRequest<Unit>
    {
        public int IdAction { get; }
        public string CodDeliveryPoint { get; }
        public bool FlgInclusion { get; }

        public CreateDeliveryPointCommand(
            int idAction,
            string codDeliveryPoint,
            bool flgInclusion)
        {
            IdAction = idAction;
            CodDeliveryPoint = codDeliveryPoint;
            FlgInclusion = flgInclusion;
        }
    }
}
