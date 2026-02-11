using FluentValidation;
using Promotions.Application.ProductDetails.Commands;

namespace Promotions.Application.ProductDetails.Validators
{
    public class CreateProductDetailCommandValidator : AbstractValidator<CreateProductDetailCommand>
    {
        public CreateProductDetailCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodProduct).NotEmpty();
            RuleFor(v => v.LevProduct).GreaterThanOrEqualTo(0);
            RuleFor(v => v.CodDisplay).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
        }
    }

    public class UpdateProductDetailCommandValidator : AbstractValidator<UpdateProductDetailCommand>
    {
        public UpdateProductDetailCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodProduct).NotEmpty();
            RuleFor(v => v.LevProduct).GreaterThanOrEqualTo(0);
            RuleFor(v => v.CodDisplay).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
        }
    }

    public class DeleteProductDetailCommandValidator : AbstractValidator<DeleteProductDetailCommand>
    {
        public DeleteProductDetailCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodProduct).NotEmpty();
            RuleFor(v => v.LevProduct).GreaterThanOrEqualTo(0);
            RuleFor(v => v.CodDisplay).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
        }
    }
}
