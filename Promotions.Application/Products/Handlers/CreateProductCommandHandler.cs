using MediatR;
using Promotions.Application.Products.Commands;

using Promotions.Domain.Products;
using Promotions.Domain.ProductDetails;
using Promotions.Domain.Articles;
using Promotions.Domain.Measures;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.Products.Commands.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new PromoProduct(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay,
                request.CodDiv
            );

            product.UpdateQuantities(request.QtyEstimated, request.NumMeasure, request.CodMeasure);
            product.UpdateDiscounts(request.PerceDiscount1, request.PerceDiscount2);

            foreach (var detailDto in request.Details)
            {
                var detail = new PromoProductDetail(
                    request.IdAction,
                    request.CodProduct,
                    request.LevProduct,
                    request.CodDisplay,
                    detailDto.CodNode,
                    detailDto.CodDiv,
                    detailDto.FlgInclusion
                );

                foreach (var articleDto in detailDto.Articles)
                {
                    var article = new PromoArticle(
                        idAction: request.IdAction,
                        codProduct: request.CodProduct,
                        levProduct: request.LevProduct,
                        codDisplay: request.CodDisplay,
                        codDiv: articleDto.CodDiv,
                        codNode: articleDto.CodNode,
                        codNode1: articleDto.CodNode1,
                        codNode2: articleDto.CodNode2,
                        codNodeN: articleDto.CodNodeN
                    );
                    detail.AddArticle(article);
                }
                product.AddDetail(detail);
            }

            try
            {
                // Check if product already exists
                var existingProduct = await _unitOfWork.Products.GetByIdAsync(
                    request.IdAction,
                    request.CodProduct,
                    request.LevProduct,
                    request.CodDisplay);

                if (existingProduct != null)
                {
                    // Product already exists for this action, skip creation
                    return Unit.Value;
                }

                await _unitOfWork.Products.AddAsync(product);

                // Handle Measure Fields
                if (request.MeasureFields != null && request.MeasureFields.Any() && !string.IsNullOrEmpty(request.CodMeasure))
                {
                    foreach (var field in request.MeasureFields)
                    {
                        var measureField = new PromoMeasureField(
                            request.CodDiv,
                            request.CodMeasure,
                            field.FieldName,
                            field.Formula
                        );
                        await _unitOfWork.MeasureFields.AddAsync(measureField);
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Product: {ex.Message} {ex.InnerException?.Message}");
            }

            return Unit.Value; 
        }
    }
}
