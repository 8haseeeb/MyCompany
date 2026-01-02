using Promotions.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<PromoProduct?> GetByIdAsync(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay);

        Task<List<PromoProduct>> GetByActionAsync(int idAction);
        Task<List<PromoProduct>> GetAllAsync();

        Task AddAsync(PromoProduct product);
        Task UpdateAsync(PromoProduct product);
        Task DeleteAsync(PromoProduct product);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
