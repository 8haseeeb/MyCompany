using Microsoft.EntityFrameworkCore;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Domain.PromoActions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class PromoActionRepository : IPromoActionRepository
    {
        private readonly PromotionsDbContext _context;

        public PromoActionRepository(PromotionsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PromoAction action)
        {
            _context.PromoActions.Add(action);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PromoAction action)
        {
            _context.PromoActions.Update(action);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PromoAction action)
        {
            _context.PromoActions.Remove(action);
            await _context.SaveChangesAsync();
        }

        public async Task<PromoAction?> GetByIdAsync(int idAction)
        {
            return await _context.PromoActions.FindAsync(idAction);
        }

        public async Task<List<PromoAction>> GetAllAsync()
        {
            return await _context.PromoActions.ToListAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
          => await _context.SaveChangesAsync(cancellationToken);
    }
}
