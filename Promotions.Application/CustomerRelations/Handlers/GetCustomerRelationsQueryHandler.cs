using MediatR;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.CustomerRelations.Interfaces;

namespace Promotions.Application.CustomerRelations.Queries
{
    public class GetCustomerRelationsQueryHandler
        : IRequestHandler<GetCustomerRelationsQuery, List<CustomerRelationDto>>
    {
        private readonly ICustomerRelationRepository _repository;

        public GetCustomerRelationsQueryHandler(
            ICustomerRelationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CustomerRelationDto>> Handle(
            GetCustomerRelationsQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(x => new CustomerRelationDto
            {
                CodHier = x.CodHier,
                CodDiv = x.CodDiv,
                CodNode = x.CodNode,
                IdLevel = x.IdLevel,
                DteStart = x.DteStart,
                CodParentNode = x.CodParentNode,
                DteEnd = x.DteEnd
            }).ToList();
        }
    }
}
