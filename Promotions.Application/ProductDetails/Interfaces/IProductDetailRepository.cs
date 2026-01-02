using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promotions.Domain.ProductDetails;

namespace Promotions.Application.ProductDetails.Interfaces
{
    public interface IProductDetailRepository
    {
        Task<PromoProductDetail?> GetByIdAsync(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay,
            string codNode,
            string codDiv);

        Task<List<PromoProductDetail>> GetByActionAsync(int idAction);
        Task<List<PromoProductDetail>> GetAllAsync();

        Task AddAsync(PromoProductDetail entity);
        Task UpdateAsync(PromoProductDetail entity);
        Task DeleteAsync(PromoProductDetail entity);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
