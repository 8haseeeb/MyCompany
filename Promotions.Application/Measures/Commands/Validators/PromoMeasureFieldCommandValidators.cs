using FluentValidation;
using Promotions.Application.Measures.Commands;

namespace Promotions.Application.Measures.Validators
{
    public class CreatePromoMeasureFieldCommandValidator : AbstractValidator<CreatePromoMeasureFieldCommand>
    {
        public CreatePromoMeasureFieldCommandValidator()
        {
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodMeasure).NotEmpty();
            RuleFor(v => v.FieldName).NotEmpty().MaximumLength(100);
            RuleFor(v => v.Formula).NotEmpty();
        }
    }

    public class UpdatePromoMeasureFieldCommandValidator : AbstractValidator<UpdatePromoMeasureFieldCommand>
    {
        public UpdatePromoMeasureFieldCommandValidator()
        {
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodMeasure).NotEmpty();
            RuleFor(v => v.FieldName).NotEmpty();
            RuleFor(v => v.Formula).NotEmpty();
        }
    }

    public class DeletePromoMeasureFieldCommandValidator : AbstractValidator<DeletePromoMeasureFieldCommand>
    {
        public DeletePromoMeasureFieldCommandValidator()
        {
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodMeasure).NotEmpty();
            RuleFor(v => v.FieldName).NotEmpty();
        }
    }
}
