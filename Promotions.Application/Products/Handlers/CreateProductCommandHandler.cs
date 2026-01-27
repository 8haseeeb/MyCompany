using MediatR;
using Promotions.Application.Products.Interfaces;
using Promotions.Domain.Products;
using Promotions.Application.Interfaces;
using Promotions.Domain.ProductDetails;
using Promotions.Domain.Articles;
using Promotions.Domain.Measures;

namespace Promotions.Application.Products.Commands.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
    {
        private readonly IProductRepository _repository;
        private readonly IPromoMeasureFieldRepository _measureFieldRepository;

        public CreateProductCommandHandler(IProductRepository repository, IPromoMeasureFieldRepository measureFieldRepository)
        {
            _repository = repository;
            _measureFieldRepository = measureFieldRepository;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new PromoProduct
            {
                IdAction = request.IdAction,
                CodProduct = request.CodProduct,
                LevProduct = request.LevProduct,
                CodDisplay = request.CodDisplay,
                CodDiv = request.CodDiv,
                QtyEstimated = request.QtyEstimated,
                PerceDiscount1 = request.PerceDiscount1,
                PerceDiscount2 = request.PerceDiscount2,
                NumMeasure = request.NumMeasure,
                CodMeasure = request.CodMeasure,
                Details = request.Details.Select(d => new PromoProductDetail
                {
                    IdAction = request.IdAction,
                    CodProduct = request.CodProduct,
                    LevProduct = request.LevProduct,
                    CodDisplay = request.CodDisplay,
                    CodNode = d.CodNode,
                    CodDiv = d.CodDiv,
                    FlgInclusion = d.FlgInclusion,
                    Articles = d.Articles.Select(a => new PromoArticle
                    {
                        IdAction = request.IdAction,
                        CodProduct = request.CodProduct,
                        LevProduct = request.LevProduct,
                        CodDisplay = request.CodDisplay,
                        CodDiv = a.CodDiv,
                        CodNode = a.CodNode,
                        CodNode1 = a.CodNode1,
                        CodNode2 = a.CodNode2,
                        CodNodeN = a.CodNodeN
                    }).ToList()
                }).ToList()
            };

            try
            {
                await _repository.AddAsync(product);

                // Handle Measure Fields
                if (request.MeasureFields != null && request.MeasureFields.Any() && !string.IsNullOrEmpty(request.CodMeasure))
                {
                    foreach (var field in request.MeasureFields)
                    {
                        var measureField = new PromoMeasureField
                        {
                            CodDiv = request.CodDiv,
                            CodMeasure = request.CodMeasure,
                            FieldName = field.FieldName,
                            Formula = field.Formula
                        };
                        await _measureFieldRepository.AddAsync(measureField, cancellationToken);
                    }
                }

                await _repository.SaveChangesAsync(cancellationToken);
                await _measureFieldRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // This is temporary for debugging 500 errors
                throw new Exception($"Failed to create Product: {ex.Message} {ex.InnerException?.Message}");
            }

            return Unit.Value; 
        }
    }
}
