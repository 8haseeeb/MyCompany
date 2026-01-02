using MediatR;

namespace Promotions.Application.Products.Commands
{
    public record DeleteProductCommand(
        int IdAction,
        string CodProduct,
        int LevProduct,
        string CodDisplay
    ) : IRequest<Unit>;
}
