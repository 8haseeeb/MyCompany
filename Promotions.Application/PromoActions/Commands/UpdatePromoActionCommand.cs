using MediatR;
using System;

namespace Promotions.Application.PromoActions.Commands
{
    public record UpdatePromoActionCommand(
        int IdAction,
        string Name,
        DateTime? DteStartSellIn,
        DateTime? DteEndSellIn,
        DateTime? DteStartSellOut,
        DateTime? DteEndSellOut,
        string? DocumentKey,
        DateTime? DteToShost,
        int? LevParticipants
    ) : IRequest<Unit>;
}
