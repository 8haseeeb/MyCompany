using FluentValidation;
using Promotions.Application.DeliveryPoints.Commands;

namespace Promotions.Application.DeliveryPoints.Commands.Validators
{
    public class CreateDeliveryPointCommandValidator : AbstractValidator<CreateDeliveryPointCommand>
    {
        public CreateDeliveryPointCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodDeliveryPoint).NotEmpty().MaximumLength(50);
            RuleFor(v => v.CodHier).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
        }
    }

    public class UpdateDeliveryPointCommandValidator : AbstractValidator<UpdateDeliveryPointCommand>
    {
        public UpdateDeliveryPointCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodDeliveryPoint).NotEmpty();
        }
    }

    public class DeleteDeliveryPointCommandValidator : AbstractValidator<DeleteDeliveryPointCommand>
    {
        public DeleteDeliveryPointCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodDeliveryPoint).NotEmpty();
        }
    }
}
