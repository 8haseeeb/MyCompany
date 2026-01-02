using MediatR;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.CustomerRelations.Interfaces;

namespace Promotions.Application.CustomerRelations.Queries.Handlers
{
    public class GetCustomerRelationByIdQueryHandler
        : IRequestHandler<GetCustomerRelationByIdQuery, CustomerRelationDto>
    {
        private readonly ICustomerRelationRepository _repository;

        public GetCustomerRelationByIdQueryHandler(
            ICustomerRelationRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerRelationDto> Handle(
            GetCustomerRelationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart);

            if (entity == null)
                throw new Exception("Customer Relation not found");

            return new CustomerRelationDto
            {
                CodHier = entity.CodHier,
                CodDiv = entity.CodDiv,
                CodNode = entity.CodNode,
                IdLevel = entity.IdLevel,
                DteStart = entity.DteStart,
                CodParentNode = entity.CodParentNode,
                DteEnd = entity.DteEnd
            };
        }
    }
}
