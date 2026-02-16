using NSubstitute;
using Promotions.Application.PromoArticles.Commands;
using Promotions.Application.PromoArticles.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Articles;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.PromoArticles.Handler
{
    public class DeletePromoArticleCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeletePromoArticleCommandHandler _handler;

        public DeletePromoArticleCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeletePromoArticleCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_DeletePromoArticle_WhenExists()
        {
            // --- ARRANGE ---
            var command = new DeletePromoArticleCommand("D1", "N1");
            var existingArticle = new PromoArticle(1, "P1", 1, "DISP1", "D1", "N1", null, null, null);
            _unitOfWork.PromoArticles.GetByIdAsync("D1", "N1").Returns(existingArticle);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.PromoArticles.Received(1).DeleteAsync(existingArticle);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new DeletePromoArticleCommand("D1", "N1");
            _unitOfWork.PromoArticles.GetByIdAsync("D1", "N1").Returns((PromoArticle)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
