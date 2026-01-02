using MediatR;

public record DeleteProductDetailCommand(
    int IdAction,
    string CodProduct,
    int LevProduct,
    string CodDisplay,
    string CodNode,
    string CodDiv
) : IRequest<Unit>;
