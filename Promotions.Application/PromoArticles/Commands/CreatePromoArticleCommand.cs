using MediatR;

namespace Promotions.Application.PromoArticles.Commands
{
    public record CreatePromoArticleCommand(
        int IdAction,
        string CodProduct,
        int LevProduct,
        string CodDisplay,
        string CodDiv,
        string CodNode,
        string? CodNode1,
        string? CodNode2,
        string? CodNodeN
    ) : IRequest<Unit>;
}
