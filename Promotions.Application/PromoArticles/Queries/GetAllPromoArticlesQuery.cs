using MediatR;
using Promotions.Application.PromoArticles.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.PromoArticles.Queries
{
    public record GetAllPromoArticlesQuery
        : IRequest<List<PromoArticleDto>>;
}
