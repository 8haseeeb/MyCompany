using MediatR;
using Promotions.Application.CustomerRelations.Dtos;

namespace Promotions.Application.CustomerRelations.Queries
{
    public record GetCustomerRelationsQuery(int? IdAction = null)
        : IRequest<List<CustomerRelationDto>>;
}
