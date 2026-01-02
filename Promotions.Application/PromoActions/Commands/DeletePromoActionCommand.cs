using MediatR;

namespace Promotions.Application.PromoActions.Commands
{
    public record DeletePromoActionCommand(int IdAction)
        : IRequest<Unit>;
}
