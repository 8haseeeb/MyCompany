using MediatR;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.ProductDetails.Interfaces;

namespace Promotions.Application.ProductDetails.Queries.Handlers
{
    public class GetProductDetailsByActionQueryHandler
        : IRequestHandler<GetProductDetailsByActionQuery, List<ProductDetailDto>>
    {
        private readonly IProductDetailRepository _repository;

        public GetProductDetailsByActionQueryHandler(IProductDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductDetailDto>> Handle(
            GetProductDetailsByActionQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _repository.GetByActionAsync(request.IdAction);

            return entities.Select(entity => new ProductDetailDto
            {
                IdAction = entity.IdAction,
                CodProduct = entity.CodProduct,
                LevProduct = entity.LevProduct,
                CodDisplay = entity.CodDisplay,
                CodNode = entity.CodNode,
                CodDiv = entity.CodDiv,
                FlgInclusion = entity.FlgInclusion
            }).ToList();
        }
    }
}
