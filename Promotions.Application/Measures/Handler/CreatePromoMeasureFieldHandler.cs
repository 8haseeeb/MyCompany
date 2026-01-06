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
        var entity = new PromoMeasureField
        {
            IdAction = request.IdAction,
            CodProduct = request.CodProduct,
            LevProduct = request.LevProduct,
            CodDisplay = request.CodDisplay,
            CodDiv = request.CodDiv,
            CodMeasure = request.CodMeasure,
            FieldName = request.FieldName,
            Formula = request.Formula
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
