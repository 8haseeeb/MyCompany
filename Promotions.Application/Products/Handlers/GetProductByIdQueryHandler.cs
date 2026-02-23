using MediatR;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Interfaces;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.PromoArticles.Dtos;
using Promotions.Application.Measures.Dtos;

namespace Promotions.Application.Products.Queries.Handlers
{
    public class GetProductByIdQueryHandler
        : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;
        private readonly IPromoMeasureFieldRepository _measureFieldRepository;

        public GetProductByIdQueryHandler(IProductRepository repository, IPromoMeasureFieldRepository measureFieldRepository)
        {
            _repository = repository;
            _measureFieldRepository = measureFieldRepository;
        }

        public async Task<ProductDto> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            var measureFields = product.CodMeasure != null 
                ? await _measureFieldRepository.GetByMeasureAsync(product.CodMeasure, cancellationToken)
                : new List<Promotions.Domain.Measures.PromoMeasureField>();

            return new ProductDto
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
                }).ToList(),
                MeasureFields = measureFields.Select(f => new PromoMeasureFieldDto
                {
                    CodDiv = f.CodDiv,
                    CodMeasure = f.CodMeasure,
                    FieldName = f.FieldName,
                    Formula = f.Formula
                }).ToList()
            };
        }
    }
}
