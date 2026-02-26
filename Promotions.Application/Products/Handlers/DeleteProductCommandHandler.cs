using MediatR;
using Microsoft.Extensions.Logging;
using Promotions.Application.Products.Commands;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;



namespace Promotions.Application.Products.Commands.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit> 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken) 
        {
            _logger.LogInformation("[DeleteProduct] Starting. IdAction: {IdAction}, CodProduct: {CodProduct}",
                request.IdAction, request.CodProduct);

            var product = await _unitOfWork.Products.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
            {
                _logger.LogError("[DeleteProduct] Product not found. IdAction: {IdAction}, CodProduct: {CodProduct}",
                    request.IdAction, request.CodProduct);
                throw new KeyNotFoundException("Product not found");
            }

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[DeleteProduct] Product deleted successfully. IdAction: {IdAction}, CodProduct: {CodProduct}",
                request.IdAction, request.CodProduct);

            return Unit.Value; 
        }
    }
}
