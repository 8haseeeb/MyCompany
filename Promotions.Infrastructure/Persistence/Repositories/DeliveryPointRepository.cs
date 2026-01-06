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
            await _context.DeliveryPoints.AddAsync(deliveryPoint);
            // SaveChangesAsync is typically called by a Unit of Work or a higher layer
            // if it's removed from individual repository methods.
            // If immediate saving is desired, uncomment the line below:
            // await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(PromoDeliveryPoint deliveryPoint)
        {
            _context.DeliveryPoints.Update(deliveryPoint);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PromoDeliveryPoint deliveryPoint)
        {
            _context.DeliveryPoints.Remove(deliveryPoint);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
