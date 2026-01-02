using MediatR;
using Promotions.Application.Products.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.Products.Queries
{
    public record GetProductsByActionQuery(
        int IdAction
    ) : IRequest<List<ProductDto>>;
}
