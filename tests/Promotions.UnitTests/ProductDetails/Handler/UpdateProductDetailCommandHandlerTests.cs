using NSubstitute;
using Promotions.Application.ProductDetails.Commands;
using Promotions.Application.ProductDetails.Handlers;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Domain.ProductDetails;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.ProductDetails.Handler
{
    public class UpdateProductDetailCommandHandlerTests
    {
        private readonly IProductDetailRepository _repo;
        private readonly UpdateProductDetailCommandHandler _handler;

        public UpdateProductDetailCommandHandlerTests()
        {
            _repo = Substitute.For<IProductDetailRepository>();
            _handler = new UpdateProductDetailCommandHandler(_repo);
        }

        [Fact]
        public async Task Handle_Should_UpdateProductDetail_WhenExists()
        {
            // --- ARRANGE ---
            var command = new UpdateProductDetailCommand(1, "P1", 1, "DISP1", "N1", "D1", false);
            var existingDetail = new PromoProductDetail(1, "P1", 1, "DISP1", "N1", "D1", true);
            _repo.GetByIdAsync(1, "P1", 1, "DISP1", "N1", "D1").Returns(existingDetail);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            Assert.False(existingDetail.FlgInclusion);
            await _repo.Received(1).UpdateAsync(existingDetail);
            await _repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new UpdateProductDetailCommand(1, "P1", 1, "DISP1", "N1", "D1", false);
            _repo.GetByIdAsync(1, "P1", 1, "DISP1", "N1", "D1").Returns((PromoProductDetail)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
