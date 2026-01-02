using MediatR;

public record CreateProductDetailCommand(
    int IdAction,
    string CodProduct,
    int LevProduct,
    string CodDisplay,
    string CodNode,
    string CodDiv,
    bool FlgInclusion
) : IRequest<Unit>;
