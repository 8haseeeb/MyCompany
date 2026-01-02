using MediatR;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Interfaces;

namespace Promotions.Application.Products.Queries.Handlers
{
    public class GetProductByIdQueryHandler
        : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
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
                CodMeasure = product.CodMeasure
            };
        }
    }
}
