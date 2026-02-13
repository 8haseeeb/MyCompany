using MediatR;
using Promotions.Application.PromoArticles.Commands;

using Promotions.Domain.Articles;
using Promotions.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.PromoArticles.Commands.Handlers
{
    public class CreatePromoArticleCommandHandler
        : IRequestHandler<CreatePromoArticleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromoArticleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.PromoArticles.AddAsync(article);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
