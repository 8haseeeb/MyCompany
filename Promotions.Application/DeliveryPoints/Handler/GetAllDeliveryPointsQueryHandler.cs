using MediatR;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.DeliveryPoints.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.DeliveryPoints.Queries
{
    public class GetAllDeliveryPointsQueryHandler : IRequestHandler<GetAllDeliveryPointsQuery, List<DeliveryPointDto>>
    {
        private readonly IDeliveryPointRepository _repository;

        public GetAllDeliveryPointsQueryHandler(IDeliveryPointRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DeliveryPointDto>> Handle(GetAllDeliveryPointsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(x => new DeliveryPointDto
            {
                IdAction = x.IdAction,
                CodDeliveryPoint = x.CodDeliveryPoint,
                FlgInclusion = x.FlgInclusion
            }).ToList();
        }
    }
}
