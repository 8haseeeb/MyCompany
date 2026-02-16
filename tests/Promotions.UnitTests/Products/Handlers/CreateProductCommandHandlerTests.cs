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
    public class CreateProductCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateProductCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_CreateProduct_When_NotExists()
        {
            // --- ARRANGE ---
            var command = new CreateProductCommand(
                IdAction: 1,
                CodProduct: "P1",
                LevProduct: 1,
                CodDisplay: "DISP1",
                CodDiv: "D1",
                QtyEstimated: 100,
                PerceDiscount1: 10,
                PerceDiscount2: null,
                NumMeasure: 1,
                CodMeasure: "M1",
                Details: new List<Promotions.Application.ProductDetails.Dtos.AtomicCreateProductDetailDto>(),
                MeasureFields: new List<Promotions.Application.Products.Dtos.CreatePromoMeasureFieldDto>()
            );

            _unitOfWork.Products.GetByIdAsync(1, "P1", 1, "DISP1").Returns((PromoProduct)null!);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.Products.Received(1).AddAsync(Arg.Any<PromoProduct>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_Should_Skip_When_ProductAlreadyExists()
        {
            // --- ARRANGE ---
            var command = new CreateProductCommand(
                1, "P1", 1, "DISP1", "D1", 100, 10, null, 1, "M1", 
                new List<Promotions.Application.ProductDetails.Dtos.AtomicCreateProductDetailDto>(), 
                new List<Promotions.Application.Products.Dtos.CreatePromoMeasureFieldDto>()
            );

            var existingProduct = new PromoProduct(1, "P1", 1, "DISP1", "D1");
            _unitOfWork.Products.GetByIdAsync(1, "P1", 1, "DISP1").Returns(existingProduct);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.Products.DidNotReceive().AddAsync(Arg.Any<PromoProduct>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
