using MediatR;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Domain.Articles;

namespace Promotions.Application.PromoArticles.Queries.Handlers
{
    public class GetPromoArticleByIdQueryHandler
        : IRequestHandler<GetPromoArticleByIdQuery, PromoArticle?>
    {
        private readonly IPromoArticleRepository _repository;

        public GetPromoArticleByIdQueryHandler(IPromoArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PromoArticle?> Handle(
            GetPromoArticleByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(
                request.CodDiv,
                request.CodNode);
        }
    }
}
