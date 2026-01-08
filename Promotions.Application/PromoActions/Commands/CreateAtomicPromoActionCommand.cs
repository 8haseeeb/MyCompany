using MediatR;
using Promotions.Application.PromoActions.Dtos;

namespace Promotions.Application.PromoActions.Commands
{
    public record CreateAtomicPromoActionCommand(AtomicCreatePromoActionDto Dto) : IRequest<Unit>;
}
