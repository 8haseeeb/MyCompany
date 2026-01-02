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
            var article = new PromoArticle
            {
                CodDiv = request.CodDiv,
                CodNode = request.CodNode,
                CodNode1 = request.CodNode1,
                CodNode2 = request.CodNode2,
                CodNodeN = request.CodNodeN
            };

            await _repository.AddAsync(article);
            await _repository.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
