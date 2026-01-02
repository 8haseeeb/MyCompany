using Promotions.Domain.DeliveryPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.DeliveryPoints.Interfaces
{
    public interface IDeliveryPointRepository
    {
        Task<PromoDeliveryPoint?> GetByIdAsync(int idAction, string codDeliveryPoint);
        Task<List<PromoDeliveryPoint>> GetByActionIdAsync(int idAction);
        Task<List<PromoDeliveryPoint>> GetAllAsync();
        Task AddAsync(PromoDeliveryPoint deliveryPoint);
        Task UpdateAsync(PromoDeliveryPoint deliveryPoint);
        Task DeleteAsync(PromoDeliveryPoint deliveryPoint);
    }
}
