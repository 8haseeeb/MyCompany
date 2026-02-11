using FluentValidation;
using Promotions.Application.Products.Commands;

namespace Promotions.Application.Products.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodProduct).NotEmpty();
            RuleFor(v => v.LevProduct).GreaterThanOrEqualTo(0);
            RuleFor(v => v.CodDisplay).NotEmpty();

            RuleFor(v => v.PerceDiscount1)
                .InclusiveBetween(0, 100).When(v => v.PerceDiscount1.HasValue);
            
            RuleFor(v => v.PerceDiscount2)
                .InclusiveBetween(0, 100).When(v => v.PerceDiscount2.HasValue);
        }
    }

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodProduct).NotEmpty();
            RuleFor(v => v.LevProduct).GreaterThanOrEqualTo(0);
            RuleFor(v => v.CodDisplay).NotEmpty();
        }
    }
}
