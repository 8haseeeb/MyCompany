using MediatR;
using Promotions.Application.Products.Interfaces;
using Promotions.Domain.Products;

namespace Promotions.Application.Products.Commands.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit> // ✅ Add <Unit>
    {
        private readonly IProductRepository _repository;

        public CreateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
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
                CodMeasure = request.CodMeasure
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value; 
        }
    }
}
