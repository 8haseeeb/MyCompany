using MediatR;

namespace Promotions.Application.Measures.Commands
{
    public record CreatePromoMeasureFieldCommand(
        string CodDiv,
        string CodMeasure,
        string FieldName,
        string Formula
    ) : IRequest<Unit>;  
}

