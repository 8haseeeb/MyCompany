using Promotions.Application.Interfaces;
using Promotions.Domain.Measures;
using Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class PromoMeasureFieldRepository : IPromoMeasureFieldRepository
    {
        private readonly PromotionsDbContext _context;

        public PromoMeasureFieldRepository(PromotionsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PromoMeasureField entity, CancellationToken cancellationToken)
        {
            _context.PromoMeasureFields.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
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
            string codMeasure,
            string codDiv,
            string fieldName,
            CancellationToken cancellationToken)
        {
           
            return await _context.PromoMeasureFields.FindAsync(
                new object[] { codDiv, codMeasure, fieldName },
                cancellationToken);
        }

   
        public async Task<List<PromoMeasureField>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.PromoMeasureFields.ToListAsync(cancellationToken);
        }

     
        public async Task UpdateAsync(PromoMeasureField entity, CancellationToken cancellationToken)
        {
            _context.PromoMeasureFields.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

       
        public async Task DeleteAsync(PromoMeasureField entity, CancellationToken cancellationToken)
        {
            _context.PromoMeasureFields.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
