using MediatR;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Domain.Articles;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoArticles.Queries.Handlers
{
    public class GetAllPromoArticlesQueryHandler
        : IRequestHandler<GetAllPromoArticlesQuery, List<PromoArticle>>
    {
        private readonly IPromoArticleRepository _articleRepo;
        private readonly IProductDetailRepository _detailRepo;

        public GetAllPromoArticlesQueryHandler(
            IPromoArticleRepository articleRepo,
            IProductDetailRepository detailRepo)
        {
            _articleRepo = articleRepo;
            _detailRepo = detailRepo;
        }

        public async Task<List<PromoArticle>> Handle(
            GetAllPromoArticlesQuery request,
            CancellationToken cancellationToken)
        {
            // Try to get articles from the master table first
            var articles = await _articleRepo.GetAllAsync();
            
            // Also get articles derived from product details (hierarchy data)
            var details = await _detailRepo.GetAllAsync();
            var derivedArticles = details
                .GroupBy(d => new { d.CodDiv, d.CodNode })
                .Select(g => 
                {
                    var d = g.First();
                    // Map to PromoArticle domain model (dummying action/product fields for master list view)
                    return new PromoArticle(
                        d.IdAction,
                        d.CodProduct,
                        d.LevProduct,
                        d.CodDisplay,
                        d.CodDiv,
                        d.CodNode,
                        null, null, null
                    );
                });

            // Combine and return unique articles (by CodDiv + CodNode)
            return articles
                .Concat(derivedArticles)
                .GroupBy(a => new { a.CodDiv, a.CodNode })
                .Select(g => g.First())
                .ToList();
        }
    }
}
