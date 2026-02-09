using AutoMapper;
using MediatR;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.PromoActions.Queries;
using Promotions.Application.CustomerRelations.Dtos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Handler
{
    public class GetPromoActionByIdQueryHandler : IRequestHandler<GetPromoActionByIdQuery, PromoActionDetailDto?>
    {
        private readonly IPromoActionRepository _repository;
        private readonly Promotions.Application.CustomerRelations.Interfaces.ICustomerRelationRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetPromoActionByIdQueryHandler(
            IPromoActionRepository repository,
            Promotions.Application.CustomerRelations.Interfaces.ICustomerRelationRepository customerRepository,
            IMapper mapper)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<PromoActionDetailDto?> Handle(GetPromoActionByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.IdAction);

            if (entity == null) return null;

            // Logic: Derive Contractor from the first Participant found
            var firstParticipant = entity.Participants?.FirstOrDefault();
            var customerRelation = firstParticipant != null 
                ? (await _customerRepository.GetByNodeAndDivAsync(firstParticipant.CodNode!, firstParticipant.CodDiv!)).FirstOrDefault() 
                : null;

            var dto = _mapper.Map<PromoActionDetailDto>(entity);

            if (customerRelation != null)
            {
                dto.CustomerRelation = _mapper.Map<CustomerRelationDetailDto>(customerRelation);
            }

            // Map Measure Fields for each product
            foreach (var productDto in dto.Products)
            {
                var measureFields = await _repository.GetMeasureFieldsByMeasureAsync(productDto.CodDiv, productDto.CodMeasure ?? string.Empty);
                productDto.MeasureFields = _mapper.Map<List<Promotions.Application.Measures.Dtos.PromoMeasureFieldDto>>(measureFields);
            }

            return dto;
        }
    }
}
