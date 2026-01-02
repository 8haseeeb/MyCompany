using MediatR;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Products.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Products.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public GetAllProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(x => new ProductDto
            {
                IdAction = x.IdAction,
                CodProduct = x.CodProduct,
                LevProduct = x.LevProduct,
                CodDisplay = x.CodDisplay,
                CodDiv = x.CodDiv,
                QtyEstimated = x.QtyEstimated,
                PerceDiscount1 = x.PerceDiscount1,
                PerceDiscount2 = x.PerceDiscount2,
                NumMeasure = x.NumMeasure,
                CodMeasure = x.CodMeasure
            }).ToList();
        }
    }
}
