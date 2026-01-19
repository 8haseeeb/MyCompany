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
                FlgInclusion = x.FlgInclusion,
                CodHier = x.CodHier ?? x.Relation?.CodHier ?? string.Empty,
                CodDiv = x.CodDiv ?? x.Relation?.CodDiv ?? string.Empty,
                CodNode = x.CodNode ?? x.Relation?.CodNode ?? string.Empty,
                IdLevel = x.IdLevel != 0 ? x.IdLevel : (x.Relation?.IdLevel ?? 0),
                DteStart = x.DteStart != default ? x.DteStart : (x.Relation?.DteStart ?? default)
            }).ToList();
        }
    }
}
