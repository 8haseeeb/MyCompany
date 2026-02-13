using MediatR;
using Promotions.Application.Products.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Promotions.Application.Products.Commands;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.Products.Commands.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

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
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update product: {ex.Message} {ex.InnerException?.Message}");
            }

            return Unit.Value;
        }
    }
}
