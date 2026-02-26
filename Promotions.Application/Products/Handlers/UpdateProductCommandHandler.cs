using MediatR;
using Microsoft.Extensions.Logging;
using Promotions.Application.Products.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.Products.Commands.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[UpdateProduct] Starting. IdAction: {IdAction}, CodProduct: {CodProduct}",
                request.IdAction, request.CodProduct);

            var product = await _unitOfWork.Products.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
            {
                _logger.LogError("[UpdateProduct] Product not found. IdAction: {IdAction}, CodProduct: {CodProduct}",
                    request.IdAction, request.CodProduct);
                throw new KeyNotFoundException("Product not found");
            }

            // Update allowed fields
            if (request.CodDiv != null)
                product.UpdateDivision(request.CodDiv);
                
            product.UpdateQuantities(
                request.QtyEstimated ?? product.QtyEstimated,
                request.NumMeasure ?? product.NumMeasure,
                request.CodMeasure ?? product.CodMeasure
            );

            product.UpdateDiscounts(
                request.PerceDiscount1 ?? product.PerceDiscount1,
                request.PerceDiscount2 ?? product.PerceDiscount2
            );

            try
            {
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("[UpdateProduct] Product updated successfully. IdAction: {IdAction}, CodProduct: {CodProduct}",
                    request.IdAction, request.CodProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateProduct] Failed to update product. IdAction: {IdAction}, CodProduct: {CodProduct}. Error: {Error}",
                    request.IdAction, request.CodProduct, ex.Message);
                throw new Exception($"Failed to update product: {ex.Message} {ex.InnerException?.Message}");
            }

            return Unit.Value;
        }
    }
}
