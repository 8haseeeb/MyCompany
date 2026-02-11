using FluentValidation;
using Promotions.Application.Products.Commands;

namespace Promotions.Application.Products.Commands.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(v => v.IdAction)
                .NotEmpty().WithMessage("Action ID is required.");

            RuleFor(v => v.CodProduct)
                .NotEmpty().WithMessage("Product Code is required.")
                .MaximumLength(50).WithMessage("Product Code must not exceed 50 characters.");

            RuleFor(v => v.CodDiv)
                .NotEmpty().WithMessage("Division Code is required.");

            RuleFor(v => v.QtyEstimated)
                .GreaterThanOrEqualTo(0).WithMessage("Estimated Quantity must be 0 or greater.");

            RuleFor(v => v.PerceDiscount1)
                .InclusiveBetween(0, 100).When(v => v.PerceDiscount1.HasValue)
                .WithMessage("Discount 1 must be between 0 and 100.");

            RuleFor(v => v.PerceDiscount2)
                .InclusiveBetween(0, 100).When(v => v.PerceDiscount2.HasValue)
                .WithMessage("Discount 2 must be between 0 and 100.");
        }
    }
}
