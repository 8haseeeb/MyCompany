using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.PromoArticles.Commands;


namespace Promotions.Application.PromoArticles.Commands.Handlers
{
    public class DeletePromoArticleCommandHandler
        : IRequestHandler<DeletePromoArticleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromoArticleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeletePromoArticleCommand request, CancellationToken ct)
        {
            var article = await _unitOfWork.PromoArticles.GetByIdAsync(
                request.CodDiv, request.CodNode);

            if (article == null)
                throw new KeyNotFoundException("PromoArticle not found");

            await _unitOfWork.PromoArticles.DeleteAsync(article);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
