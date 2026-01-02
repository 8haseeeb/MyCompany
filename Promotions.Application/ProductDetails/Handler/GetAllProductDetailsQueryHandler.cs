using MediatR;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Application.ProductDetails.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.ProductDetails.Handler
{
    public class GetAllProductDetailsQueryHandler : IRequestHandler<GetAllProductDetailsQuery, List<ProductDetailDto>>
    {
        private readonly IProductDetailRepository _repository;

        public GetAllProductDetailsQueryHandler(IProductDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductDetailDto>> Handle(GetAllProductDetailsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(x => new ProductDetailDto
            {
                IdAction = x.IdAction,
                CodProduct = x.CodProduct,
                LevProduct = x.LevProduct,
                CodDisplay = x.CodDisplay,
                CodNode = x.CodNode,
                CodDiv = x.CodDiv,
                FlgInclusion = x.FlgInclusion
            }).ToList();
        }
    }
}
