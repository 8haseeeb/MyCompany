using MediatR;

namespace Promotions.Application.Products.Commands
{
    public record CreateProductCommand(
        int IdAction,
        string CodProduct,
        int LevProduct,
        string CodDisplay,
        string CodDiv,
        decimal QtyEstimated,
        decimal? PerceDiscount1,
        decimal? PerceDiscount2,
        decimal? NumMeasure,
        string? CodMeasure
    ) : IRequest<Unit>;
}
