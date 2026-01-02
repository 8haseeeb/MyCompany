using MediatR;
using Promotions.Application.DeliveryPoints.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.DeliveryPoints.Queries
{
    public class GetAllDeliveryPointsQuery : IRequest<List<DeliveryPointDto>>
    {
    }
}
