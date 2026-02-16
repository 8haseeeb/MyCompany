using NSubstitute;
using Promotions.Application.PromoArticles.Commands;
using Promotions.Application.PromoArticles.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Articles;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.PromoArticles.Handler
{
    public class CreatePromoArticleCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreatePromoArticleCommandHandler _handler;

        public CreatePromoArticleCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreatePromoArticleCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_CreatePromoArticle()
        {
            // --- ARRANGE ---
            var command = new CreatePromoArticleCommand(
                1, "P1", 1, "DISP1", "D1", "N1", null, null, null
            );

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.PromoArticles.Received(1).AddAsync(Arg.Any<PromoArticle>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
