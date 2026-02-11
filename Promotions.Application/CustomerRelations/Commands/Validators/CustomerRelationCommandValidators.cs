using FluentValidation;
using Promotions.Application.CustomerRelations.Commands;

namespace Promotions.Application.CustomerRelations.Validators
{
    public class CreateCustomerRelationCommandValidator : AbstractValidator<CreateCustomerRelationCommand>
    {
        public CreateCustomerRelationCommandValidator()
        {
            RuleFor(v => v.CodHier).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
            RuleFor(v => v.IdLevel).GreaterThan(0);
            RuleFor(v => v.DteStart).NotEmpty();
            RuleFor(v => v.CodParentNode).NotEmpty();
        }
    }

    public class UpdateCustomerRelationCommandValidator : AbstractValidator<UpdateCustomerRelationCommand>
    {
        public UpdateCustomerRelationCommandValidator()
        {
            RuleFor(v => v.CodHier).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
            RuleFor(v => v.IdLevel).GreaterThan(0);
            RuleFor(v => v.DteStart).NotEmpty();
        }
    }

    public class DeleteCustomerRelationCommandValidator : AbstractValidator<DeleteCustomerRelationCommand>
    {
        public DeleteCustomerRelationCommandValidator()
        {
            RuleFor(v => v.CodHier).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
            RuleFor(v => v.IdLevel).GreaterThan(0);
            RuleFor(v => v.DteStart).NotEmpty();
        }
    }
}
