using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promotions.Domain.ProductDetails;

namespace Promotions.Application.ProductDetails.Interfaces
{
    public interface IProductDetailRepository : Promotions.Domain.Shared.IRepository<PromoProductDetail>
    {
        Task<PromoProductDetail?> GetByIdAsync(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay,
            string codNode,
            string codDiv);

        Task<List<PromoProductDetail>> GetByActionAsync(int idAction);
    }
}
