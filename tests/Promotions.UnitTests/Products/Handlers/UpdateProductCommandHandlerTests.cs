using NSubstitute;
using Promotions.Application.Products.Commands.Handlers;
using Promotions.Application.Products.Commands;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.Products;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace Promotions.UnitTests.Products.Handlers
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            // 1. Arrange: Create mock for IUnitOfWork
            _unitOfWork = Substitute.For<IUnitOfWork>();
            
            // 2. Arrange: Initialize handler with the mock
            _handler = new UpdateProductCommandHandler(_unitOfWork, NullLogger<UpdateProductCommandHandler>.Instance);
        }

        [Fact]
        public async Task Handle_Should_UpdateProduct_When_ProductExists()
        {
            // --- ARRANGE ---
            var command = new UpdateProductCommand(
                IdAction: 1, 
                CodProduct: "P1", 
                LevProduct: 1,
                CodDisplay: "DISP1",
                CodDiv: "UPDATED_DIV",
                QtyEstimated: 500,
                PerceDiscount1: null,
                PerceDiscount2: null,
                NumMeasure: null,
                CodMeasure: null
            );

            // Using domain constructor
            var existingProduct = new PromoProduct(1, "P1", 1, "DISP1", "OLD_DIV");

            // Mocking the repository GetById equivalent with correct 4-argument signature
            _unitOfWork.Products.GetByIdAsync(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<string>()).Returns(existingProduct);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            // Verify that CodDiv was updated on the existing object
            Assert.Equal("UPDATED_DIV", existingProduct.CodDiv);
            
            // Verify that SaveChangesAsync was called exactly once
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
