using FluentValidation;
using Promotions.Application.PromoActions.Commands;
using System.Collections.Generic;

namespace Promotions.Application.PromoActions.Validators
{
    public class CreateAtomicPromoActionCommandValidator : AbstractValidator<CreateAtomicPromoActionCommand>
    {
        public CreateAtomicPromoActionCommandValidator()
        {
            RuleFor(v => v.Dto).NotNull().WithMessage("Action data is required.");
            RuleFor(v => v.Dto.Name).NotEmpty().WithMessage("Promotion Name is required.");
            RuleFor(v => v.Dto.CodDiv).NotEmpty().WithMessage("Division Code is required.");
            
            RuleFor(v => v.Dto.Products).NotNull();
            RuleFor(v => v.Dto.Participants).NotNull();
            RuleFor(v => v.Dto.DeliveryPoints).NotNull();
        }
    }

    public class DeletePromoActionCommandValidator : AbstractValidator<DeletePromoActionCommand>
    {
        public DeletePromoActionCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty().WithMessage("Action ID is required.");
        }
    }
}
