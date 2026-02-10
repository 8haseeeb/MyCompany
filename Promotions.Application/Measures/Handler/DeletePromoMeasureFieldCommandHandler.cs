using MediatR;
using Promotions.Application.Interfaces;
using Promotions.Domain.Measures;

namespace Promotions.Application.Measures.Commands
{
    public class DeletePromoMeasureFieldHandler
        : IRequestHandler<DeletePromoMeasureFieldCommand, Unit>
    {
        private readonly IPromoMeasureFieldRepository _repository;

        public DeletePromoMeasureFieldHandler(IPromoMeasureFieldRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeletePromoMeasureFieldCommand request, CancellationToken cancellationToken)
        {
           
            var entity = await _repository.GetByIdAsync(
                request.CodDiv,
                request.CodMeasure,
                request.FieldName,
                cancellationToken);


            if (entity == null)
                throw new Exception("Record not found"); 

            // Delete
            await _repository.DeleteAsync(entity);

            return Unit.Value;
        }
    }
}
