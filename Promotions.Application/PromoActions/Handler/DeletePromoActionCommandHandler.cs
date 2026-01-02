using MediatR;
using Promotions.Application.PromoActions.Interfaces;

namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class DeletePromoActionCommandHandler
        : IRequestHandler<DeletePromoActionCommand, Unit>
    {
        private readonly IPromoActionRepository _repository;

        public DeletePromoActionCommandHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeletePromoActionCommand request, CancellationToken cancellationToken)
        {
            var action = await _repository.GetByIdAsync(request.IdAction);

            if (action == null)
                throw new KeyNotFoundException("Promotion not found");

            await _repository.DeleteAsync(action);
            await _repository.SaveChangesAsync(cancellationToken);


            return Unit.Value;
        }
    }
}
