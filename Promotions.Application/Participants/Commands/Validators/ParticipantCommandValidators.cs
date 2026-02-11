using FluentValidation;
using Promotions.Application.Participants.Commands;

namespace Promotions.Application.Participants.Commands.Validators
{
    public class CreateParticipantCommandValidator : AbstractValidator<CreateParticipantCommand>
    {
        public CreateParticipantCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodParticipant).NotEmpty().MaximumLength(50);
            RuleFor(v => v.CodHier).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
        }
    }

    public class UpdateParticipantCommandValidator : AbstractValidator<UpdateParticipantCommand>
    {
        public UpdateParticipantCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodParticipant).NotEmpty();
        }
    }

    public class DeleteParticipantCommandValidator : AbstractValidator<DeleteParticipantCommand>
    {
        public DeleteParticipantCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodParticipant).NotEmpty();
        }
    }
}
