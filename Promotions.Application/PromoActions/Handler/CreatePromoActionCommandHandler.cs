using MediatR;
using Promotions.Domain.PromoActions;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.PromoActions.Commands;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class CreatePromoActionCommandHandler
        : IRequestHandler<CreatePromoActionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromoActionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CreatePromoActionCommand request, CancellationToken cancellationToken)
        {
            var action = new PromoAction(request.IdAction, request.Name, request.CodDiv);
            action.UpdateBasicInfo(request.Name, request.CodDiv, request.DocumentKey, request.LevParticipants);
            action.UpdateSellInDates(request.DteStartSellIn, request.DteEndSellIn);
            action.UpdateSellOutDates(request.DteStartSellOut, request.DteEndSellOut);
            action.SetHostDate(request.DteToShost);

            await _unitOfWork.PromoActions.AddAsync(action);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

