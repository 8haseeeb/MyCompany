using MediatR;
using System.Collections.Generic;
using Promotions.Application.Participants.Dtos;
using Promotions.Application.Products.Dtos;
using Promotions.Application.DeliveryPoints.Dtos;

namespace Promotions.Application.PromoActions.Commands
{
    public record CreatePromoActionCommand(
        int IdAction,
        string Name,
        string CodDiv,
        DateTime DteStartSellIn,
        DateTime DteEndSellIn,
        DateTime DteStartSellOut,
        DateTime DteEndSellOut,
        string? DocumentKey,
        DateTime? DteToShost,
        int? LevParticipants
    ) : IRequest<Unit>;
}


