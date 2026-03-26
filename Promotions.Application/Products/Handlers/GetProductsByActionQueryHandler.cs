using MediatR;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.PromoArticles.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace Promotions.Application.Products.Queries.Handlers
{
    public class GetProductsByActionQueryHandler
        : IRequestHandler<GetProductsByActionQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public GetProductsByActionQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductDto>> Handle(
            GetProductsByActionQuery request,
            CancellationToken cancellationToken)
        {
            var products = await _repository.GetByActionAsync(request.IdAction);

            return products.Select(product => new ProductDto
            {
                IdAction = product.IdAction,
                CodProduct = product.CodProduct,
                LevProduct = product.LevProduct,
                CodDisplay = product.CodDisplay,
                CodDiv = product.CodDiv ?? string.Empty,
                QtyEstimated = product.QtyEstimated,
                PerceDiscount1 = product.PerceDiscount1,
                PerceDiscount2 = product.PerceDiscount2,
                NumMeasure = product.NumMeasure,
                CodMeasure = product.CodMeasure,
                Details = product.Details.Select(d => new ProductDetailDto
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
