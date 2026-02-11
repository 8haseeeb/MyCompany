using MediatR;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Application.ProductDetails.Commands;
using Promotions.Domain.ProductDetails;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.ProductDetails.Handlers
{
    public class CreateProductDetailCommandHandler
        : IRequestHandler<CreateProductDetailCommand, Unit>
    {
        private readonly IProductDetailRepository _repo;

        public CreateProductDetailCommandHandler(IProductDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(CreateProductDetailCommand r, CancellationToken ct)
        {
            var entity = new PromoProductDetail(
                r.IdAction,
                r.CodProduct,
                r.LevProduct,
                r.CodDisplay,
                r.CodNode,
                r.CodDiv,
                r.FlgInclusion
            );

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
