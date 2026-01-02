using MediatR;
using Promotions.Application.ProductDetails.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.ProductDetails.Queries
{
    public record GetProductDetailsByActionQuery(
        int IdAction
    ) : IRequest<List<ProductDetailDto>>;
}
