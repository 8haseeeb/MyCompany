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
                CodDiv = product.CodDiv,
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
                    Articles = d.Articles.Select(a => new PromoArticleDto
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
                    }).ToList()
                }).ToList()
            }).ToList();
        }
    }
}
