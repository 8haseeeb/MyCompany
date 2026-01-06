using MediatR;

public record CreatePromoMeasureFieldCommand(
    int IdAction,
    string CodProduct,
    int LevProduct,
    string CodDisplay,
    string CodDiv,
    string CodMeasure,
    string FieldName,
    string Formula
) : IRequest<Unit>;  
