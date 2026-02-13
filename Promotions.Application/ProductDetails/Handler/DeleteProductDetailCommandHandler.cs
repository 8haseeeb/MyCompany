using MediatR;
using Promotions.Application.ProductDetails.Commands;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.ProductDetails.Handlers
{
    public class DeleteProductDetailCommandHandler
        : IRequestHandler<DeleteProductDetailCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductDetailCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteProductDetailCommand r, CancellationToken ct)
        {
            var entity = await _unitOfWork.ProductDetails.GetByIdAsync(
                r.IdAction, r.CodProduct, r.LevProduct,
                r.CodDisplay, r.CodNode, r.CodDiv);

            if (entity == null)
                throw new KeyNotFoundException("Product Detail not found");

            await _unitOfWork.ProductDetails.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
