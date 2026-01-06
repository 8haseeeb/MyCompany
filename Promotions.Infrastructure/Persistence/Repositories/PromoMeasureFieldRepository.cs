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
            await _context.PromoMeasureFields.AddAsync(entity, cancellationToken);
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

     
        public Task UpdateAsync(PromoMeasureField entity, CancellationToken cancellationToken)
        {
            _context.PromoMeasureFields.Update(entity);
            return Task.CompletedTask;
        }

       
        public Task DeleteAsync(PromoMeasureField entity, CancellationToken cancellationToken)
        {
            _context.PromoMeasureFields.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
