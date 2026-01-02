using MediatR;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.DeliveryPoints.Interfaces;

namespace Promotions.Application.DeliveryPoints.Queries
{
    public class GetDeliveryPointByIdQueryHandler : IRequestHandler<GetDeliveryPointByIdQuery, DeliveryPointDto>
    {
        private readonly IDeliveryPointRepository _repository;

        public GetDeliveryPointByIdQueryHandler(IDeliveryPointRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeliveryPointDto> Handle(GetDeliveryPointByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.IdAction, request.CodDeliveryPoint);
            
            if (entity == null) return null!;

            return new DeliveryPointDto
            {
                IdAction = entity.IdAction,
                CodDeliveryPoint = entity.CodDeliveryPoint,
                FlgInclusion = entity.FlgInclusion
            };
        }
    }
}
