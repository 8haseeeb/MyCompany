using MediatR;
using Promotions.Application.Products.Interfaces;

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

            product.CodDiv = request.CodDiv;
            product.QtyEstimated = request.QtyEstimated;
            product.PerceDiscount1 = request.PerceDiscount1;
            product.PerceDiscount2 = request.PerceDiscount2;
            product.NumMeasure = request.NumMeasure;
            product.CodMeasure = request.CodMeasure;

            await _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
