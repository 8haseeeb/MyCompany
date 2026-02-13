using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.PromoActions.Commands;



namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class DeletePromoActionCommandHandler
        : IRequestHandler<DeletePromoActionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromoActionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeletePromoActionCommand request, CancellationToken cancellationToken)
        {
            var action = await _unitOfWork.PromoActions.GetByIdAsync(request.IdAction);

            if (action == null)
                throw new KeyNotFoundException("Promotion not found");

            await _unitOfWork.PromoActions.DeleteAsync(action);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return Unit.Value;
        }
    }
}
