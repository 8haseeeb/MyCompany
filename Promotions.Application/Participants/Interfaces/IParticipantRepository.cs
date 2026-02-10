using Promotions.Domain.Participants;

namespace Promotions.Application.Participant.Interfaces
{
    public interface IParticipantRepository : Promotions.Domain.Shared.IRepository<PromoParticipants>
    {
        Task<PromoParticipants?> GetByIdAsync(int idAction, string codParticipant);
        Task<List<PromoParticipants>> GetByActionAsync(int idAction);
    }
}
