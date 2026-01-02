using MediatR;

namespace Promotions.Application.PromoActions.Commands
{
    public record UpdatePromoActionCommand(
        int IdAction,
        string Name,
        DateTime DteEndSellIn,
        DateTime DteEndSellOut,
        string? DocumentKey,
        DateTime? DteToShost,
        int? LevParticipants
    ) : IRequest<Unit>;
}
