using MediatR;
using Promotions.Application.Measures.Commands;
using Promotions.Domain.Measures;
using Promotions.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.Measures.Handlers
{
    public class CreatePromoMeasureFieldHandler
        : IRequestHandler<CreatePromoMeasureFieldCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromoMeasureFieldHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CreatePromoMeasureFieldCommand request, CancellationToken cancellationToken)
        {
            var entity = new PromoMeasureField(
                request.CodDiv,
                request.CodMeasure,
                request.FieldName,
                request.Formula
            );

            await _unitOfWork.MeasureFields.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
