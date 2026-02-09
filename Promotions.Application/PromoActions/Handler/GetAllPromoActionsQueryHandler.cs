using AutoMapper;
using MediatR;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.PromoActions.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Handler
{
    public class GetAllPromoActionsQueryHandler : IRequestHandler<GetAllPromoActionsQuery, List<PromoActionDto>>
    {
        private readonly IPromoActionRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPromoActionsQueryHandler(IPromoActionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PromoActionDto>> Handle(GetAllPromoActionsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<PromoActionDto>>(entities);
        }
    }
}
