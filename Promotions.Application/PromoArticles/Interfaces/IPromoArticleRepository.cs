using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Domain.Articles;

namespace Promotions.Application.PromoArticles.Interfaces
{
    public interface IPromoArticleRepository : Promotions.Domain.Shared.IRepository<PromoArticle>
    {
        Task<PromoArticle?> GetByIdAsync(string codDiv, string codNode);
    }
}
