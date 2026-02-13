using MediatR;
using Promotions.Application.ProductDetails.Commands;
using Promotions.Domain.ProductDetails;
using Promotions.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.ProductDetails.Handlers
{
    public class CreateProductDetailCommandHandler
        : IRequestHandler<CreateProductDetailCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductDetailCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.ProductDetails.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
