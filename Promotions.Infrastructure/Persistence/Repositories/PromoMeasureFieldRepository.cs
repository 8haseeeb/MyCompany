using Promotions.Application.Interfaces;
using Promotions.Domain.Measures;
using Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class PromoMeasureFieldRepository : Repository<PromoMeasureField>, IPromoMeasureFieldRepository
    {
        public PromoMeasureFieldRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<List<PromoMeasureField>> GetByMeasureAsync(
            string codMeasure,
            CancellationToken cancellationToken)
        {
            return await _context.PromoMeasureFields
                .Where(x => x.CodMeasure == codMeasure)
                .ToListAsync(cancellationToken);
        }

        public async Task<PromoMeasureField?> GetByIdAsync(
            string codDiv,
            string codMeasure,
            string fieldName,
            CancellationToken cancellationToken)
        {
            return await _context.PromoMeasureFields.FindAsync(
                new object[] { codDiv, codMeasure, fieldName },
                cancellationToken);
        }

        // Standard CRUD & SaveChangesAsync (with token) are in base class
        // Note: AddAsync/UpdateAsync in base do not take CancellationToken.
        // Callers passing tokens to these specific methods will need to be updated.
    }
}
