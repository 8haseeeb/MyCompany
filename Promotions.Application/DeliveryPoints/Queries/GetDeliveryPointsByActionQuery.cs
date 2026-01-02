using MediatR;
using Promotions.Application.DeliveryPoints.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.DeliveryPoints.Queries
{
    public class GetDeliveryPointsByActionQuery
        : IRequest<List<DeliveryPointDto>>
    {
        public int IdAction { get; }

        public GetDeliveryPointsByActionQuery(int idAction)
        {
            IdAction = idAction;
        }
    }
}
