using Promotions.Domain.DeliveryPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.DeliveryPoints.Interfaces
{
    public interface IDeliveryPointRepository : Promotions.Domain.Shared.IRepository<PromoDeliveryPoint>
    {
        Task<PromoDeliveryPoint?> GetByIdAsync(int idAction, string codDeliveryPoint);
        Task<List<PromoDeliveryPoint>> GetByActionAsync(int idAction);
    }
}
