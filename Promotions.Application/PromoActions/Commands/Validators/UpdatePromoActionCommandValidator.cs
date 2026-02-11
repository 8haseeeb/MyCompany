using FluentValidation;
using Promotions.Application.PromoActions.Commands;

namespace Promotions.Application.PromoActions.Commands.Validators
{
    public class UpdatePromoActionCommandValidator : AbstractValidator<UpdatePromoActionCommand>
    {
        public UpdatePromoActionCommandValidator()
        {
            RuleFor(v => v.IdAction)
                .NotEmpty().WithMessage("Action ID is required.");

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Promotion Name is required.")
                .MaximumLength(200).WithMessage("Promotion Name must not exceed 200 characters.");

            RuleFor(v => v.DteStartSellIn)
                 .NotEmpty().WithMessage("Start Sell-In Date is required.");

            RuleFor(v => v.DteEndSellIn)
                .NotEmpty().WithMessage("End Sell-In Date is required.")
                .GreaterThanOrEqualTo(v => v.DteStartSellIn)
                .WithMessage("End Sell-In Date must be after or equal to Start Sell-In Date.");
        }
    }
}
