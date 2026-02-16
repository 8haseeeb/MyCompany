using NSubstitute;
using Promotions.Application.ProductDetails.Commands;
using Promotions.Application.ProductDetails.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.ProductDetails;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Promotions.UnitTests.ProductDetails.Handler
{
    public class DeleteProductDetailCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteProductDetailCommandHandler _handler;

        public DeleteProductDetailCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeleteProductDetailCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_DeleteProductDetail_WhenExists()
        {
            // --- ARRANGE ---
            var command = new DeleteProductDetailCommand(1, "P1", 1, "DISP1", "N1", "D1");
            var existingDetail = new PromoProductDetail(1, "P1", 1, "DISP1", "N1", "D1", true);
            _unitOfWork.ProductDetails.GetByIdAsync(1, "P1", 1, "DISP1", "N1", "D1").Returns(existingDetail);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.ProductDetails.Received(1).DeleteAsync(existingDetail);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new DeleteProductDetailCommand(1, "P1", 1, "DISP1", "N1", "D1");
            _unitOfWork.ProductDetails.GetByIdAsync(1, "P1", 1, "DISP1", "N1", "D1").Returns((PromoProductDetail)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
