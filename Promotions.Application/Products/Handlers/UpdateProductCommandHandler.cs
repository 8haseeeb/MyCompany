using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Products.Commands;
using Promotions.Domain.Products;

namespace Promotions.Application.Products.Commands.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _repository;

        public UpdateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            // Update allowed fields
            if (request.CodDiv != null)
                product.CodDiv = request.CodDiv;
                
            if (request.QtyEstimated.HasValue)
                product.QtyEstimated = request.QtyEstimated.Value;

            product.PerceDiscount1 = request.PerceDiscount1;
            product.PerceDiscount2 = request.PerceDiscount2;
            product.NumMeasure = request.NumMeasure;
            product.CodMeasure = request.CodMeasure;

            try
            {
                await _repository.UpdateAsync(product);
                await _repository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update product: {ex.Message} {ex.InnerException?.Message}");
            }

            return Unit.Value;
        }
    }
}
