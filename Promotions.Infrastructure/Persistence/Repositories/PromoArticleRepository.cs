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
            // Legacy link dropped IDACTION, so this returns empty. 
            // Better to use GetByNodesAsync after resolving nodes from Details.
            return await _context.PromoArticles
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        public async Task<List<PromoArticle>> GetByNodesAsync(List<(string codDiv, string codNode)> nodes)
        {
            if (nodes == null || !nodes.Any()) return new List<PromoArticle>();

            // For small set of nodes, we can use OR logic or fetch by Div and filter
            // Here we use a query that selects articles matching any of the pairs
            var query = _context.PromoArticles.AsQueryable();
            
            // Build the predicate: (Div == d1 && Node == n1) || (Div == d2 && Node == n2) ...
            // Simplified: fetch all in the divisions present and filter in memory if needed
            // OR use a Union approach if types are supported.
            // For now, let's use a safe Division + Node combination check if possible or fetch by divisions.
            var divisions = nodes.Select(n => n.codDiv).Distinct().ToList();
            var articlesInDivs = await _context.PromoArticles
                .Where(x => divisions.Contains(x.CodDiv))
                .ToListAsync();

            return articlesInDivs
                .Where(a => nodes.Any(n => n.codDiv == a.CodDiv && n.codNode == a.CodNode))
                .ToList();
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
