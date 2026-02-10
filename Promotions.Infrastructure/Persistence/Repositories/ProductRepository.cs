using Microsoft.EntityFrameworkCore;
using Promotions.Application.Products.Interfaces;
using Promotions.Domain.Products;
using Promotions.Infrastructure.Persistence;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : Repository<PromoProduct>, IProductRepository
    {
        public ProductRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<PromoProduct?> GetByIdAsync(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay)
        {
            return await _context.Products
                .Include(x => x.Action)
                .Include(x => x.Details)
                .ThenInclude(d => d.Articles)
                .FirstOrDefaultAsync(x =>
                    x.IdAction == idAction &&
                    x.CodProduct == codProduct &&
                    x.LevProduct == levProduct &&
                    x.CodDisplay == codDisplay);
        }

        public async Task<List<PromoProduct>> GetByActionAsync(int idAction)
        {
            return await _context.Products
                .Include(x => x.Action)
                .Include(x => x.Details)
                .ThenInclude(d => d.Articles)
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        public override async Task<List<PromoProduct>> GetAllAsync()
        {
            return await _context.Products
                .Include(x => x.Action)
                .Include(x => x.Details)
                .ThenInclude(d => d.Articles)
                .ToListAsync();
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
