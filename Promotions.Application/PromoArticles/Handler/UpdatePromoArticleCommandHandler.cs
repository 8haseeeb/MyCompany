using MediatR;
using Promotions.Application.PromoArticles.Interfaces;

namespace Promotions.Application.PromoArticles.Commands.Handlers
{
    public class UpdatePromoArticleCommandHandler
        : IRequestHandler<UpdatePromoArticleCommand, Unit>
    {
        private readonly IPromoArticleRepository _repository;

        public UpdatePromoArticleCommandHandler(IPromoArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdatePromoArticleCommand request, CancellationToken ct)
        {
            var article = await _repository.GetByIdAsync(
                request.CodDiv, request.CodNode);

            if (article == null)
                throw new KeyNotFoundException("PromoArticle not found");

            article.UpdateNodes(request.CodNode1, request.CodNode2, request.CodNodeN);

            await _repository.UpdateAsync(article);
            await _repository.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
