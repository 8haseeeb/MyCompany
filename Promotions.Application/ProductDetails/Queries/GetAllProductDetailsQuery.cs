using MediatR;
using Promotions.Application.ProductDetails.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.ProductDetails.Queries
{
    public class GetAllProductDetailsQuery : IRequest<List<ProductDetailDto>>
    {
    }
}
