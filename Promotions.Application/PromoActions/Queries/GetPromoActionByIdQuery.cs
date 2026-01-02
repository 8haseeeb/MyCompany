using MediatR;
using Promotions.Application.PromoActions.Dtos;

namespace Promotions.Application.PromoActions.Queries
{
    public record GetPromoActionByIdQuery(int IdAction)
        : IRequest<PromoActionDto?>;
}
