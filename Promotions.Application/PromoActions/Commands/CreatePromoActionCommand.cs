using MediatR;

namespace Promotions.Application.PromoActions.Commands
{
    public record CreatePromoActionCommand(
        int IdAction,
        string Name,
        string CodDiv,
        string CodContractor,
        DateTime DteStartSellIn,
        DateTime DteEndSellIn,
        DateTime DteStartSellOut,
        DateTime DteEndSellOut,
        string? DocumentKey,
        DateTime? DteToShost,
        int? LevParticipants
    ) : IRequest<Unit>;
}
