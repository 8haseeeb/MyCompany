using MediatR;
using Promotions.Application.ProductDetails.Interfaces;

public class UpdateProductDetailCommandHandler
    : IRequestHandler<UpdateProductDetailCommand, Unit>
{
    private readonly IProductDetailRepository _repo;

    public UpdateProductDetailCommandHandler(IProductDetailRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(UpdateProductDetailCommand r, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(
            r.IdAction, r.CodProduct, r.LevProduct,
            r.CodDisplay, r.CodNode, r.CodDiv);

        if (entity == null)
            throw new KeyNotFoundException("Product Detail not found");

        entity.FlgInclusion = r.FlgInclusion;

        await _repo.UpdateAsync(entity);
        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
