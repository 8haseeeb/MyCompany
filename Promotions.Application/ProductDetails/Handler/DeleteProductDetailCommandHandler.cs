using MediatR;
using Promotions.Application.ProductDetails.Interfaces;

public class DeleteProductDetailCommandHandler
    : IRequestHandler<DeleteProductDetailCommand, Unit>
{
    private readonly IProductDetailRepository _repo;

    public DeleteProductDetailCommandHandler(IProductDetailRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(DeleteProductDetailCommand r, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(
            r.IdAction, r.CodProduct, r.LevProduct,
            r.CodDisplay, r.CodNode, r.CodDiv);

        if (entity == null)
            throw new KeyNotFoundException("Product Detail not found");

        await _repo.DeleteAsync(entity);
        await _repo.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
