using NSubstitute;
using Promotions.Application.Products.Commands;
using Promotions.Application.Products.Commands.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Products;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.Products.Handlers
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeleteProductCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_DeleteProduct_WhenExists()
        {
            // --- ARRANGE ---
            var command = new DeleteProductCommand(1, "P1", 1, "DISP1");
            var existingProduct = new PromoProduct(1, "P1", 1, "DISP1", "D1");

            _unitOfWork.Products.GetByIdAsync(1, "P1", 1, "DISP1").Returns(existingProduct);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.Products.Received(1).DeleteAsync(existingProduct);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_ThrowException_WhenNotFound()
        {
            // --- ARRANGE ---
            var command = new DeleteProductCommand(1, "P1", 1, "DISP1");
            _unitOfWork.Products.GetByIdAsync(1, "P1", 1, "DISP1").Returns((PromoProduct)null!);

            // --- ACT & ASSERT ---
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
