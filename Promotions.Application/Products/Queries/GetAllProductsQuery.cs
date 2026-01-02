using MediatR;
using Promotions.Application.Products.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.Products.Queries
{
    public class GetAllProductsQuery : IRequest<List<ProductDto>>
    {
    }
}
