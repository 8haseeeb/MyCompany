using MediatR;
using Promotions.Application.PromoActions.Dtos;

namespace Promotions.Application.PromoActions.Queries
{
    public record GetAllPromoActionsQuery()
        : IRequest<List<PromoActionDto>>;
}
