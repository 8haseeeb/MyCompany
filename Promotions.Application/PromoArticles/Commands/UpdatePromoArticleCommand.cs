using MediatR;

namespace Promotions.Application.PromoArticles.Commands
{
    public record UpdatePromoArticleCommand(
        string CodDiv,
        string CodNode,
        string? CodNode1,
        string? CodNode2,
        string? CodNodeN
    ) : IRequest<Unit>;
}
