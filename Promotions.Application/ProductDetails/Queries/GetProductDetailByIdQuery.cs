using MediatR;
using Promotions.Application.ProductDetails.Dtos;

namespace Promotions.Application.ProductDetails.Queries
{
    public record GetProductDetailByIdQuery(
        int IdAction,
        string CodProduct,
        int LevProduct,
        string CodDisplay,
        string CodNode,
        string CodDiv
    ) : IRequest<ProductDetailDto>;
}
