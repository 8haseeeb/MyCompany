using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Domain.Articles;

namespace Promotions.Application.PromoArticles.Interfaces
{
    public interface IPromoArticleRepository
    {
        Task AddAsync(PromoArticle article);
        Task UpdateAsync(PromoArticle article);
        Task DeleteAsync(PromoArticle article);

        Task<PromoArticle?> GetByIdAsync(string codDiv, string codNode);
        Task<List<PromoArticle>> GetAllAsync();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
