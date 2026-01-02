using MediatR;

namespace Promotions.Application.PromoArticles.Commands
{
    public record DeletePromoArticleCommand(
        string CodDiv,
        string CodNode
    ) : IRequest<Unit>;
}
