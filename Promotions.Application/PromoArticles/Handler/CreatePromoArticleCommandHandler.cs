using MediatR;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Domain.Articles;

namespace Promotions.Application.PromoArticles.Commands.Handlers
{
    public class CreatePromoArticleCommandHandler
        : IRequestHandler<CreatePromoArticleCommand, Unit>
    {
        private readonly IPromoArticleRepository _repository;

        public CreatePromoArticleCommandHandler(IPromoArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreatePromoArticleCommand request, CancellationToken ct)
        {
            var article = new PromoArticle(
                idAction: request.IdAction,
                codProduct: request.CodProduct,
                levProduct: request.LevProduct,
                codDisplay: request.CodDisplay,
                codDiv: request.CodDiv,
                codNode: request.CodNode,
                codNode1: request.CodNode1,
                codNode2: request.CodNode2,
                codNodeN: request.CodNodeN
            );

            await _repository.AddAsync(article);
            await _repository.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
