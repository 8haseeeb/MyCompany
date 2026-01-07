using MediatR;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Domain.PromoActions;
using System.Threading;

namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class CreatePromoActionCommandHandler
        : IRequestHandler<CreatePromoActionCommand, Unit>
    {
        private readonly IPromoActionRepository _repository;

        public CreatePromoActionCommandHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreatePromoActionCommand request, CancellationToken cancellationToken)
        {
            var action = new PromoAction
            {
                IdAction = request.IdAction,
                Name = request.Name,
                CodDiv = request.CodDiv,
                CodContractor = request.CodContractor,
                DteStartSellIn = request.DteStartSellIn,
                DteEndSellIn = request.DteEndSellIn,
                DteStartSellOut = request.DteStartSellOut,
                DteEndSellOut = request.DteEndSellOut,
                DocumentKey = request.DocumentKey,
                DteToShost = request.DteToShost,
                LevParticipants = request.LevParticipants
            };

            await _repository.AddAsync(action);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
