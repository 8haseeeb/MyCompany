using MediatR;
using Promotions.Application.Products.Interfaces;

namespace Promotions.Application.Products.Commands.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit> 
    {
        private readonly IProductRepository _repository;

        public DeleteProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken) 
        {
            var product = await _repository.GetByIdAsync(
                request.IdAction,
                request.CodProduct,
                request.LevProduct,
                request.CodDisplay);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            await _repository.DeleteAsync(product);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value; 
        }
    }
}
