using MediatR;
using Promotions.Application.Products.Commands;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.Products.Commands;


namespace Promotions.Application.Products.Commands.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit> 
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken) 
        {
            var product = await _unitOfWork.Products.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value; 
        }
    }
}
