using MediatR;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Domain.Articles;
using System.Collections.Generic;

namespace Promotions.Application.PromoArticles.Queries.Handlers
{
    public class GetAllPromoArticlesQueryHandler
        : IRequestHandler<GetAllPromoArticlesQuery, List<PromoArticle>>
    {
        private readonly IPromoArticleRepository _repository;

        public GetAllPromoArticlesQueryHandler(IPromoArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PromoArticle>> Handle(
            GetAllPromoArticlesQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}
