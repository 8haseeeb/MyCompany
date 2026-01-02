using MediatR;
using Promotions.Application.CustomerRelations.Dtos;

namespace Promotions.Application.CustomerRelations.Queries
{
    public record GetCustomerRelationsQuery()
        : IRequest<List<CustomerRelationDto>>;
}
