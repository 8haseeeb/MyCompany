using MediatR;
using Promotions.Domain.Articles;

namespace Promotions.Application.PromoArticles.Queries
{
    public record GetPromoArticleByIdQuery(
        string CodDiv,
        string CodNode
    ) : IRequest<PromoArticle?>;
}
