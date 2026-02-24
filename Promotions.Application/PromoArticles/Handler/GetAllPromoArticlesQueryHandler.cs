using MediatR;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Application.PromoArticles.Dtos;
using Promotions.Domain.Articles;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoArticles.Queries.Handlers
{
    public class GetAllPromoArticlesQueryHandler
        : IRequestHandler<GetAllPromoArticlesQuery, List<PromoArticleDto>>
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

        public async Task<List<PromoArticleDto>> Handle(
            GetAllPromoArticlesQuery request,
            CancellationToken cancellationToken)
        {
            // Try to get articles from the master table first
            var articles = await _articleRepo.GetAllAsync();
            var articleDtos = articles.Select(a => new PromoArticleDto
            {
                IdAction = a.IdAction,
                CodProduct = a.CodProduct,
                LevProduct = a.LevProduct,
                CodDisplay = a.CodDisplay,
                CodDiv = a.CodDiv,
                CodNode = a.CodNode,
                CodNode1 = a.CodNode1,
                CodNode2 = a.CodNode2,
                CodNodeN = a.CodNodeN
            }).ToList();
            
            // Also get articles derived from product details (hierarchy data)
            var details = await _detailRepo.GetAllAsync();
            var derivedArticleDtos = details
                .GroupBy(d => new { d.CodDiv, d.CodNode })
                .Select(g => 
                {
                    var d = g.First();
                    return new PromoArticleDto
                    {
                        IdAction = d.IdAction,
                        CodProduct = d.CodProduct,
                        LevProduct = d.LevProduct,
                        CodDisplay = d.CodDisplay,
                        CodDiv = d.CodDiv,
                        CodNode = d.CodNode,
                        CodNode1 = null,
                        CodNode2 = null,
                        CodNodeN = "0"
                    };
                });

            // Combine and return unique articles (by CodDiv + CodNode)
            return articleDtos
                .Concat(derivedArticleDtos)
                .GroupBy(a => new { a.CodDiv, a.CodNode })
                .Select(g => g.First())
                .ToList();
        }
    }
}
