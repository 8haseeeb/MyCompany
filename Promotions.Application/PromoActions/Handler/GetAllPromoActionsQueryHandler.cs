using MediatR;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.PromoActions.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Handler
{
    public class GetAllPromoActionsQueryHandler : IRequestHandler<GetAllPromoActionsQuery, List<PromoActionDto>>
    {
        private readonly IPromoActionRepository _repository;

        public GetAllPromoActionsQueryHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PromoActionDto>> Handle(GetAllPromoActionsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(x => new PromoActionDto
            {
                IdAction = x.IdAction,
                Name = x.Name,
                CodDiv = x.CodDiv,
                CodContractor = x.CodContractor,
                DteStartSellIn = x.DteStartSellIn,
                DteEndSellIn = x.DteEndSellIn,
                DteStartSellOut = x.DteStartSellOut,
                DteEndSellOut = x.DteEndSellOut,
                DocumentKey = x.DocumentKey,
                DteToShost = x.DteToShost,
                LevParticipants = x.LevParticipants
            }).ToList();
        }
    }
}
