using Microsoft.EntityFrameworkCore;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Domain.Articles;


namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class PromoArticleRepository : Repository<PromoArticle>, IPromoArticleRepository
    {
        public PromoArticleRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<PromoArticle?> GetByIdAsync(string codDiv, string codNode)
            => await _context.PromoArticles
                .FirstOrDefaultAsync(x => x.CodDiv == codDiv && x.CodNode == codNode);

        public async Task<List<PromoArticle>> GetByActionAsync(int idAction)
        {
            return await _context.PromoArticles
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
