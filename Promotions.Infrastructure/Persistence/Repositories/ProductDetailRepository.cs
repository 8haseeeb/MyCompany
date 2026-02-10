using Microsoft.EntityFrameworkCore;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Domain.ProductDetails;
using Promotions.Infrastructure.Persistence;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class ProductDetailRepository : Repository<PromoProductDetail>, IProductDetailRepository
    {
        public ProductDetailRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<PromoProductDetail?> GetByIdAsync(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay,
            string codNode,
            string codDiv)
        {
            return await _context.ProductDetails.FirstOrDefaultAsync(x =>
                x.IdAction == idAction &&
                x.CodProduct == codProduct &&
                x.LevProduct == levProduct &&
                x.CodDisplay == codDisplay &&
                x.CodNode == codNode &&
                x.CodDiv == codDiv);
        }

        public async Task<List<PromoProductDetail>> GetByActionAsync(int idAction)
        {
            return await _context.ProductDetails
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
