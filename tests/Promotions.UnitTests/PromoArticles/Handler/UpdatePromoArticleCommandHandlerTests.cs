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
    public class UpdatePromoArticleCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdatePromoArticleCommandHandler _handler;

        public UpdatePromoArticleCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdatePromoArticleCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_UpdatePromoArticle_WhenExists()
        {
            // --- ARRANGE ---
            var command = new UpdatePromoArticleCommand(
                CodDiv: "D1", CodNode: "N1", CodNode1: "NEW1", CodNode2: null, CodNodeN: null
            );
            
            var existingArticle = new PromoArticle(1, "P1", 1, "DISP1", "D1", "N1", null, null, null);
            _unitOfWork.PromoArticles.GetByIdAsync("D1", "N1").Returns(existingArticle);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.Equal("NEW1", existingArticle.CodNode1);
            await _unitOfWork.PromoArticles.Received(1).UpdateAsync(existingArticle);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new UpdatePromoArticleCommand("D1", "N1", null, null, null);
            _unitOfWork.PromoArticles.GetByIdAsync("D1", "N1").Returns((PromoArticle)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
