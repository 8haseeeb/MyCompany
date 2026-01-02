using Microsoft.EntityFrameworkCore;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Domain.Articles;


namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class PromoArticleRepository : IPromoArticleRepository
    {
        private readonly PromotionsDbContext _context;

        public PromoArticleRepository(PromotionsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PromoArticle article)
        {
            await _context.PromoArticles.AddAsync(article);
        }

        public Task UpdateAsync(PromoArticle article)
        {
            _context.PromoArticles.Update(article);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PromoArticle article)
        {
            _context.PromoArticles.Remove(article);
            return Task.CompletedTask;
        }

        public async Task<PromoArticle?> GetByIdAsync(string codDiv, string codNode)
            => await _context.PromoArticles
                .FirstOrDefaultAsync(x => x.CodDiv == codDiv && x.CodNode == codNode);

        public async Task<List<PromoArticle>> GetAllAsync()
            => await _context.PromoArticles.ToListAsync();

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
