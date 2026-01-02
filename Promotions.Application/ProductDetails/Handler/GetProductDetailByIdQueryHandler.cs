using MediatR;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.ProductDetails.Interfaces;

namespace Promotions.Application.ProductDetails.Queries.Handlers
{
    public class GetProductDetailByIdQueryHandler
        : IRequestHandler<GetProductDetailByIdQuery, ProductDetailDto>
    {
        private readonly IProductDetailRepository _repository;

        public GetProductDetailByIdQueryHandler(IProductDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDetailDto> Handle(
            GetProductDetailByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay,
                request.CodNode,
                request.CodDiv);

            if (entity == null)
                throw new KeyNotFoundException("Product Detail not found");

            return new ProductDetailDto
            {
                IdAction = entity.IdAction,
                CodProduct = entity.CodProduct,
                LevProduct = entity.LevProduct,
                CodDisplay = entity.CodDisplay,
                CodNode = entity.CodNode,
                CodDiv = entity.CodDiv,
                FlgInclusion = entity.FlgInclusion
            };
        }
    }
}
