using MediatR;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Interfaces;

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
                CodMeasure = product.CodMeasure
            }).ToList();
        }
    }
}
