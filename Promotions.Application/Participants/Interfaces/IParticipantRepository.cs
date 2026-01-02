using Promotions.Domain.Participants;

namespace Promotions.Application.Participant.Interfaces
{
    public interface IParticipantRepository
    {
        Task<PromoParticipants?> GetByIdAsync(int idAction, string codParticipant);
        Task AddAsync(PromoParticipants participant);
        Task UpdateAsync(PromoParticipants participant);
        Task DeleteAsync(PromoParticipants participant);
        Task<List<PromoParticipants>> GetByActionIdAsync(int idAction);

        Task<List<PromoParticipants>> GetAllAsync();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
