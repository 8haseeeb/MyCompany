using MediatR;
using Promotions.Application.DeliveryPoints.Dtos;

namespace Promotions.Application.DeliveryPoints.Queries
{
    public class GetDeliveryPointByIdQuery : IRequest<DeliveryPointDto>
    {
        public int IdAction { get; }
        public string CodDeliveryPoint { get; }

        public GetDeliveryPointByIdQuery(int idAction, string codDeliveryPoint)
        {
            IdAction = idAction;
            CodDeliveryPoint = codDeliveryPoint;
        }
    }
}
