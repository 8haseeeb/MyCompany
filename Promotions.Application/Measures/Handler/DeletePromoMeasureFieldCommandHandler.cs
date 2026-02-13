using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.Measures.Commands;


namespace Promotions.Application.Measures.Commands
{
    public class DeletePromoMeasureFieldHandler
        : IRequestHandler<DeletePromoMeasureFieldCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromoMeasureFieldHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeletePromoMeasureFieldCommand request, CancellationToken cancellationToken)
        {
           
            var entity = await _unitOfWork.MeasureFields.GetByIdAsync(
                request.CodDiv,
                request.CodMeasure,
                request.FieldName,
                cancellationToken);


            if (entity == null)
                throw new Exception("Record not found"); 

            // Delete
            await _unitOfWork.MeasureFields.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
