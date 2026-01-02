using MediatR;
using Promotions.Application.Interfaces;
using Promotions.Domain.Measures;

public class CreatePromoMeasureFieldHandler
    : IRequestHandler<CreatePromoMeasureFieldCommand, Unit>  // <-- add Unit
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

        await _repository.AddAsync(entity, cancellationToken);

        return Unit.Value;
    }
}
