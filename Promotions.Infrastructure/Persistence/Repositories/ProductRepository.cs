using Microsoft.EntityFrameworkCore;
using Promotions.Application.Products.Interfaces;
using Promotions.Domain.Products;
using Promotions.Infrastructure.Persistence;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PromotionsDbContext _context;

        public ProductRepository(PromotionsDbContext context)
        {
            _context = context;
        }

        public async Task<PromoProduct?> GetByIdAsync(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay)
        {
            return await _context.Products.FirstOrDefaultAsync(x =>
                x.IdAction == idAction &&
                x.CodProduct == codProduct &&
                x.LevProduct == levProduct &&
                x.CodDisplay == codDisplay);
        }

        public async Task<List<PromoProduct>> GetByActionAsync(int idAction)
        {
            return await _context.Products
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        public async Task<List<PromoProduct>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task AddAsync(PromoProduct product)
        {
            await _context.Products.AddAsync(product);
        }

        public Task UpdateAsync(PromoProduct product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PromoProduct product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
           => await _context.SaveChangesAsync(cancellationToken);
    }
}
