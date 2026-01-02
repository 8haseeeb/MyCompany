using MediatR;

public record CreatePromoMeasureFieldCommand(
    string CodDiv,
    string CodMeasure,
    string FieldName,
    string Formula
) : IRequest<Unit>;  
