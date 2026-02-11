using FluentValidation;
using Promotions.Application.PromoActions.Commands;
using System;

namespace Promotions.Application.PromoActions.Commands.Validators
{
    public class CreatePromoActionCommandValidator : AbstractValidator<CreatePromoActionCommand>
    {
        public CreatePromoActionCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Promotion Name is required.")
                .MaximumLength(200).WithMessage("Promotion Name must not exceed 200 characters.");

            RuleFor(v => v.CodDiv)
                .NotEmpty().WithMessage("Division Code is required.");

            RuleFor(v => v.DteStartSellIn)
                .NotEmpty().WithMessage("Start Sell-In Date is required.");

            RuleFor(v => v.DteEndSellIn)
                .NotEmpty().WithMessage("End Sell-In Date is required.")
                .GreaterThanOrEqualTo(v => v.DteStartSellIn)
                .WithMessage("End Sell-In Date must be after or equal to Start Sell-In Date.");

            RuleFor(v => v.DteStartSellOut)
                .NotEmpty().WithMessage("Start Sell-Out Date is required.");

            RuleFor(v => v.DteEndSellOut)
                .NotEmpty().WithMessage("End Sell-Out Date is required.")
                .GreaterThanOrEqualTo(v => v.DteStartSellOut)
                .WithMessage("End Sell-Out Date must be after or equal to Start Sell-Out Date.");
        }
    }
}
