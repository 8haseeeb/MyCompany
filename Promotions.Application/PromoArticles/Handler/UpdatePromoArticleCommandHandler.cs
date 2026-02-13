using MediatR;
using Promotions.Application.PromoArticles.Commands;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.PromoArticles.Commands.Handlers
{
    public class UpdatePromoArticleCommandHandler
        : IRequestHandler<UpdatePromoArticleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePromoArticleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdatePromoArticleCommand request, CancellationToken ct)
        {
            var article = await _unitOfWork.PromoArticles.GetByIdAsync(
                request.CodDiv, request.CodNode);

            if (article == null)
                throw new KeyNotFoundException("PromoArticle not found");

            article.UpdateNodes(request.CodNode1, request.CodNode2, request.CodNodeN);

            await _unitOfWork.PromoArticles.UpdateAsync(article);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
