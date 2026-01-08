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
            await _context.PromoActions.AddAsync(action);
        }

        public Task UpdateAsync(PromoAction action)
        {
            _context.PromoActions.Update(action);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PromoAction action)
        {
            _context.PromoActions.Remove(action);
            return Task.CompletedTask;
        }

        public async Task<PromoAction?> GetByIdAsync(int idAction)
        {
            return await _context.PromoActions
                .Include(x => x.Participants)
                .Include(x => x.DeliveryPoints)
                .Include(x => x.Products)
                    .ThenInclude(p => p.MeasureFields)
                .Include(x => x.Products)
                    .ThenInclude(p => p.Details)
                        .ThenInclude(d => d.Articles)
                .FirstOrDefaultAsync(x => x.IdAction == idAction);
        }

        public async Task<List<PromoAction>> GetAllAsync()
        {
            return await _context.PromoActions.ToListAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
          => await _context.SaveChangesAsync(cancellationToken);

        public async Task<Promotions.Application.Common.Interfaces.IAtomicTransaction> BeginTransactionAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return new EfAtomicTransaction(transaction);
        }
    }
}


