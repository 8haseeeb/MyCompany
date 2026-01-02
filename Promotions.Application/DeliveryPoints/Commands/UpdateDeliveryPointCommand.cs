using MediatR;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class UpdateDeliveryPointCommand : IRequest<Unit>
    {
        public int IdAction { get; }
        public string CodDeliveryPoint { get; }
        public bool FlgInclusion { get; }

        public UpdateDeliveryPointCommand(
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
