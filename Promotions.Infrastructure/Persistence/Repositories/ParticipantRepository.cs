using Promotions.Domain.Participants;
using Promotions.Application.Participant.Interfaces;
using Microsoft.EntityFrameworkCore;
using Promotions.Infrastructure.Persistence;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class ParticipantRepository : Repository<PromoParticipants>, IParticipantRepository
    {
        public ParticipantRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<PromoParticipants?> GetByIdAsync(int idAction, string codParticipant)
        {
            return await _context.Participants
                .Include(p => p.Relation)
                .FirstOrDefaultAsync(p =>
                    p.IdAction == idAction &&
                    p.CodParticipant == codParticipant);
        }

        public async Task<List<PromoParticipants>> GetByActionAsync(int idAction)
        {
            return await _context.Participants
                .Include(p => p.Relation)
                .Where(p => p.IdAction == idAction)
                .ToListAsync();
        }

        public override async Task<List<PromoParticipants>> GetAllAsync()
        {
            return await _context.Participants
                .Include(p => p.Relation)
                .ToListAsync();
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
