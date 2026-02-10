using Microsoft.EntityFrameworkCore;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Domain.DeliveryPoints;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class DeliveryPointRepository : Repository<PromoDeliveryPoint>, IDeliveryPointRepository
    {
        public DeliveryPointRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<PromoDeliveryPoint?> GetByIdAsync(int idAction, string codDeliveryPoint)
        {
            return await _context.DeliveryPoints
                .Include(x => x.Relation)
                .FirstOrDefaultAsync(x => x.IdAction == idAction && x.CodDeliveryPoint == codDeliveryPoint);
        }

        public async Task<List<PromoDeliveryPoint>> GetByActionAsync(int idAction)
        {
            // Note: Interface says GetByActionAsync, implementation was GetByActionIdAsync? 
            // Checking interface... interface has GetByActionAsync. 
            // Implementation had GetByActionIdAsync. Fixing implementation name to match interface.
            return await _context.DeliveryPoints
                .Include(x => x.Relation)
                .Where(x => x.IdAction == idAction)
                .ToListAsync();
        }

        public override async Task<List<PromoDeliveryPoint>> GetAllAsync()
        {
            return await _context.DeliveryPoints
                .Include(x => x.Relation)
                .ToListAsync();
        }
    }
}
