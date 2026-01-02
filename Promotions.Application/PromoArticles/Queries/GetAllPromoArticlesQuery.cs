using MediatR;
using Promotions.Domain.Articles;
using System.Collections.Generic;

namespace Promotions.Application.PromoArticles.Queries
{
    public record GetAllPromoArticlesQuery
        : IRequest<List<PromoArticle>>;
}
