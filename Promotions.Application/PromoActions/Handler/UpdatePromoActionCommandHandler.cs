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

            action.Name = request.Name;
            
            if (request.DteStartSellIn.HasValue)
                action.DteStartSellIn = request.DteStartSellIn.Value;
                
            if (request.DteEndSellIn.HasValue)
                action.DteEndSellIn = request.DteEndSellIn.Value;
                
            if (request.DteStartSellOut.HasValue)
                action.DteStartSellOut = request.DteStartSellOut.Value;
                
            if (request.DteEndSellOut.HasValue)
                action.DteEndSellOut = request.DteEndSellOut.Value;

            action.DocumentKey = request.DocumentKey;
            action.DteToShost = request.DteToShost;
            action.LevParticipants = request.LevParticipants;

            await _repository.UpdateAsync(action);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
