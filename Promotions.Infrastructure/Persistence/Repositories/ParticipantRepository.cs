using Promotions.Domain.Participants;
using Promotions.Application.Participant.Interfaces;
using Microsoft.EntityFrameworkCore;
using Promotions.Infrastructure.Persistence;

namespace Promotions.Infrastructure.Participant
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly PromotionsDbContext _context;

        public ParticipantRepository(PromotionsDbContext context)
        {
            _context = context;
        }

        public async Task<PromoParticipants?> GetByIdAsync(int idAction, string codParticipant)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(p =>
                    p.IdAction == idAction &&
                    p.CodParticipant == codParticipant);
        }

        public async Task AddAsync(PromoParticipants participant)
        {
            await _context.Participants.AddAsync(participant);
        }

        public Task UpdateAsync(PromoParticipants participant)
        {
            _context.Participants.Update(participant);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PromoParticipants participant)
        {
            _context.Participants.Remove(participant);
            return Task.CompletedTask;
        }

        public async Task<List<PromoParticipants>> GetByActionIdAsync(int idAction)
        {
            return await _context.Participants
                .Where(p => p.IdAction == idAction)
                .ToListAsync();
        }

        public async Task<List<PromoParticipants>> GetAllAsync()
        {
            return await _context.Participants.ToListAsync();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
