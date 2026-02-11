using FluentValidation;
using Promotions.Application.PromoArticles.Commands;

namespace Promotions.Application.PromoArticles.Validators
{
    public class CreatePromoArticleCommandValidator : AbstractValidator<CreatePromoArticleCommand>
    {
        public CreatePromoArticleCommandValidator()
        {
            RuleFor(v => v.IdAction).NotEmpty();
            RuleFor(v => v.CodProduct).NotEmpty();
            RuleFor(v => v.CodDisplay).NotEmpty();
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
        }
    }

    public class UpdatePromoArticleCommandValidator : AbstractValidator<UpdatePromoArticleCommand>
    {
        public UpdatePromoArticleCommandValidator()
        {
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
        }
    }

    public class DeletePromoArticleCommandValidator : AbstractValidator<DeletePromoArticleCommand>
    {
        public DeletePromoArticleCommandValidator()
        {
            RuleFor(v => v.CodDiv).NotEmpty();
            RuleFor(v => v.CodNode).NotEmpty();
        }
    }
}
