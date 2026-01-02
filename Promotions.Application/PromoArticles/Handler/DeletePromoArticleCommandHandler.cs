using MediatR;
using Promotions.Application.PromoArticles.Interfaces;

namespace Promotions.Application.PromoArticles.Commands.Handlers
{
    public class DeletePromoArticleCommandHandler
        : IRequestHandler<DeletePromoArticleCommand, Unit>
    {
        private readonly IPromoArticleRepository _repository;

        public DeletePromoArticleCommandHandler(IPromoArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeletePromoArticleCommand request, CancellationToken ct)
        {
            var article = await _repository.GetByIdAsync(
                request.CodDiv, request.CodNode);

            if (article == null)
                throw new KeyNotFoundException("PromoArticle not found");

            await _repository.DeleteAsync(article);
            await _repository.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
