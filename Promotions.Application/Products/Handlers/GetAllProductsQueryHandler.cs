using MediatR;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Products.Queries;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.PromoArticles.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Products.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public GetAllProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(x => new ProductDto
            {
                IdAction = x.IdAction,
                CodProduct = x.CodProduct,
                LevProduct = x.LevProduct,
                CodDisplay = x.CodDisplay,
                CodDiv = x.CodDiv,
                QtyEstimated = x.QtyEstimated,
                PerceDiscount1 = x.PerceDiscount1,
                PerceDiscount2 = x.PerceDiscount2,
                NumMeasure = x.NumMeasure,
                CodMeasure = x.CodMeasure,
                Details = x.Details.Select(d => new ProductDetailDto
                {
                    IdAction = d.IdAction,
                    CodProduct = d.CodProduct,
                    LevProduct = d.LevProduct,
                    CodDisplay = d.CodDisplay,
                    CodNode = d.CodNode,
                    CodDiv = d.CodDiv,
                    FlgInclusion = d.FlgInclusion,
                    Articles = d.Article != null ? new List<PromoArticleDto>
                    {
                        new PromoArticleDto
                        {
                            IdAction = d.Article.IdAction,
                            CodProduct = d.Article.CodProduct,
                            LevProduct = d.Article.LevProduct,
                            CodDisplay = d.Article.CodDisplay,
                            CodDiv = d.Article.CodDiv,
                            CodNode = d.Article.CodNode,
                            CodNode1 = d.Article.CodNode1,
                            CodNode2 = d.Article.CodNode2,
                            CodNodeN = d.Article.CodNodeN
                        }
                    } : new List<PromoArticleDto>()
                }).ToList()
            }).ToList();
        }
    }
}
