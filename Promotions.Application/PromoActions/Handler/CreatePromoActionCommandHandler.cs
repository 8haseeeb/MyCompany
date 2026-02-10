using MediatR;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Participants;
using Promotions.Domain.Products;
using Promotions.Domain.DeliveryPoints;
using System.Threading;
using System.Linq;

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
            var action = new PromoAction(request.IdAction, request.Name, request.CodDiv);
            action.UpdateBasicInfo(request.Name, request.CodDiv, request.DocumentKey, request.LevParticipants);
            action.UpdateSellInDates(request.DteStartSellIn, request.DteEndSellIn);
            action.UpdateSellOutDates(request.DteStartSellOut, request.DteEndSellOut);
            action.SetHostDate(request.DteToShost);

            await _repository.AddAsync(action);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

