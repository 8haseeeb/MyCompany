using MediatR;
using Promotions.Application.Interfaces;
using Promotions.Application.Measures.Commands;
using Promotions.Domain.Measures;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Measures.Handlers
{
    public class CreatePromoMeasureFieldHandler
        : IRequestHandler<CreatePromoMeasureFieldCommand, Unit>
    {
        private readonly IPromoMeasureFieldRepository _repository;

        public CreatePromoMeasureFieldHandler(IPromoMeasureFieldRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreatePromoMeasureFieldCommand request, CancellationToken cancellationToken)
        {
            var entity = new PromoMeasureField(
                request.CodDiv,
                request.CodMeasure,
                request.FieldName,
                request.Formula
            );

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
