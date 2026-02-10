using MediatR;
using Promotions.Application.PromoActions.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

            action.UpdateBasicInfo(request.Name, action.CodDiv, request.DocumentKey, request.LevParticipants);
            
            action.UpdateSellInDates(
                request.DteStartSellIn ?? action.DteStartSellIn,
                request.DteEndSellIn ?? action.DteEndSellIn
            );

            action.UpdateSellOutDates(
                request.DteStartSellOut ?? action.DteStartSellOut,
                request.DteEndSellOut ?? action.DteEndSellOut
            );

            action.SetHostDate(request.DteToShost);

            await _repository.UpdateAsync(action);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
