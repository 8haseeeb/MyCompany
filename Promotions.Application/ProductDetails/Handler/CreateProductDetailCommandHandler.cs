using MediatR;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Domain.ProductDetails;

public class CreateProductDetailCommandHandler
    : IRequestHandler<CreateProductDetailCommand, Unit>
{
    private readonly IProductDetailRepository _repo;

    public CreateProductDetailCommandHandler(IProductDetailRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(CreateProductDetailCommand r, CancellationToken ct)
    {
        var entity = new PromoProductDetail
        {
            IdAction = r.IdAction,
            CodProduct = r.CodProduct,
            LevProduct = r.LevProduct,
            CodDisplay = r.CodDisplay,
            CodNode = r.CodNode,
            CodDiv = r.CodDiv,
            FlgInclusion = r.FlgInclusion
        };

        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
