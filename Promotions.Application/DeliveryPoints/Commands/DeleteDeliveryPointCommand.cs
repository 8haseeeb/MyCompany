using MediatR;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class DeleteDeliveryPointCommand : IRequest<Unit>
    {
        public int IdAction { get; }
        public string CodDeliveryPoint { get; }

        public DeleteDeliveryPointCommand(
            int idAction,
            string codDeliveryPoint)
        {
            IdAction = idAction;
            CodDeliveryPoint = codDeliveryPoint;
        }
    }
}
