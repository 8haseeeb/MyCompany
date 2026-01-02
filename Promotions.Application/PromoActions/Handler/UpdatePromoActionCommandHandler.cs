using MediatR;
using Promotions.Application.PromoActions.Interfaces;

namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class UpdatePromoActionCommandHandler
        : IRequestHandler<UpdatePromoActionCommand, Unit>
    {
        private readonly IPromoActionRepository _repository;

        public UpdatePromoActionCommandHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdatePromoActionCommand request, CancellationToken cancellationToken)
        {
            var action = await _repository.GetByIdAsync(request.IdAction);

            if (action == null)
                throw new KeyNotFoundException("Promotion not found");

            action.Name = request.Name;
            action.DteEndSellIn = request.DteEndSellIn;
            action.DteEndSellOut = request.DteEndSellOut;
            action.DocumentKey = request.DocumentKey;
            action.DteToShost = request.DteToShost;
            action.LevParticipants = request.LevParticipants;

            await _repository.UpdateAsync(action);
            await _repository.SaveChangesAsync(cancellationToken);


            return Unit.Value;
        }
    }
}
