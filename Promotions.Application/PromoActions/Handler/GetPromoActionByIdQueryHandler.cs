using MediatR;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.PromoActions.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Handler
{
    public class GetPromoActionByIdQueryHandler : IRequestHandler<GetPromoActionByIdQuery, PromoActionDto?>
    {
        private readonly IPromoActionRepository _repository;

        public GetPromoActionByIdQueryHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PromoActionDto?> Handle(GetPromoActionByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.IdAction);

            if (entity == null) return null;

            return new PromoActionDto
            {
                IdAction = entity.IdAction,
                Name = entity.Name,
                CodDiv = entity.CodDiv,
                CodContractor = entity.CodContractor,
                DteStartSellIn = entity.DteStartSellIn,
                DteEndSellIn = entity.DteEndSellIn,
                DteStartSellOut = entity.DteStartSellOut,
                DteEndSellOut = entity.DteEndSellOut,
                DocumentKey = entity.DocumentKey,
                DteToShost = entity.DteToShost,
                LevParticipants = entity.LevParticipants
            };
        }
    }
}
