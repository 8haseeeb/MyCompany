using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.Measures.Commands
{
    public class UpdatePromoMeasureFieldHandler
        : IRequestHandler<UpdatePromoMeasureFieldCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePromoMeasureFieldHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UpdatePromoMeasureFieldCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.MeasureFields.GetByIdAsync(
                request.CodDiv,
                request.CodMeasure,
                request.FieldName,
                cancellationToken);

            if (entity == null)
                throw new Exception("Promo Measure Field not found");

            entity.UpdateFormula(request.Formula);

            await _unitOfWork.MeasureFields.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
