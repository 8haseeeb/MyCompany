using MediatR;
using Promotions.Application.Products.Dtos;

namespace Promotions.Application.Products.Queries
{
    public record GetProductByIdQuery(
        int IdAction,
        string CodProduct,
        int LevProduct,
        string CodDisplay
    ) : IRequest<ProductDto>;
}
