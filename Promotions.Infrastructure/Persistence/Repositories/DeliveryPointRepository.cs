using Microsoft.EntityFrameworkCore;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Domain.DeliveryPoints;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class DeliveryPointRepository : IDeliveryPointRepository
    {
        private readonly PromotionsDbContext _context;

        public DeliveryPointRepository(PromotionsDbContext context)
        {
            _context = context;
        }

        public async Task<PromoDeliveryPoint?> GetByIdAsync(int idAction, string codDeliveryPoint)
        {
            return await _context.DeliveryPoints.FindAsync(idAction, codDeliveryPoint);
        }

        public async Task<List<PromoDeliveryPoint>> GetByActionIdAsync(int idAction)
        {
            return await _context.DeliveryPoints
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        public async Task<List<PromoDeliveryPoint>> GetAllAsync()
        {
            return await _context.DeliveryPoints.ToListAsync();
        }

        public async Task AddAsync(PromoDeliveryPoint deliveryPoint)
        {
            _context.DeliveryPoints.Add(deliveryPoint);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PromoDeliveryPoint deliveryPoint)
        {
            _context.DeliveryPoints.Update(deliveryPoint);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PromoDeliveryPoint deliveryPoint)
        {
            _context.DeliveryPoints.Remove(deliveryPoint);
            await _context.SaveChangesAsync();
        }
    }
}
