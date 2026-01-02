using MediatR;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.DeliveryPoints.Interfaces;

namespace Promotions.Application.DeliveryPoints.Queries
{
    public class GetDeliveryPointsByActionQueryHandler
        : IRequestHandler<GetDeliveryPointsByActionQuery, List<DeliveryPointDto>>
    {
        private readonly IDeliveryPointRepository _repository;

        public GetDeliveryPointsByActionQueryHandler(
            IDeliveryPointRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DeliveryPointDto>> Handle(
            GetDeliveryPointsByActionQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _repository.GetByActionIdAsync(request.IdAction);

            // ENTITY → DTO mapping
            return entities.Select(x => new DeliveryPointDto
            {
                IdAction = x.IdAction,
                CodDeliveryPoint = x.CodDeliveryPoint,
                FlgInclusion = x.FlgInclusion
            }).ToList();
        }
    }
}
