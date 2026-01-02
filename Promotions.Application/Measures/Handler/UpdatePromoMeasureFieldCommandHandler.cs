using MediatR;
using Promotions.Application.Interfaces;

namespace Promotions.Application.Measures.Commands
{
    public class UpdatePromoMeasureFieldHandler
        : IRequestHandler<UpdatePromoMeasureFieldCommand, Unit>
    {
        private readonly IPromoMeasureFieldRepository _repository;

        public UpdatePromoMeasureFieldHandler(
            IPromoMeasureFieldRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            UpdatePromoMeasureFieldCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(
                request.CodMeasure,
                request.CodDiv,
                request.FieldName,
                cancellationToken);

            if (entity == null)
                throw new Exception("Promo Measure Field not found");

            entity.UpdateFormula(request.Formula);

            await _repository.UpdateAsync(entity, cancellationToken);

            return Unit.Value;
        }
    }
}
