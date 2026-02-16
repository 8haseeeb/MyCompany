using NSubstitute;
using Promotions.Application.ProductDetails.Commands;
using Promotions.Application.ProductDetails.Handlers;
using Promotions.Application.Common.Interfaces;
using Promotions.Domain.ProductDetails;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace Promotions.UnitTests.ProductDetails.Handler
{
    public class CreateProductDetailCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateProductDetailCommandHandler _handler;

        public CreateProductDetailCommandHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateProductDetailCommandHandler(_unitOfWork);
        }

        [Fact]
        public async Task Handle_Should_CreateProductDetail()
        {
            // --- ARRANGE ---
            var command = new CreateProductDetailCommand(1, "P1", 1, "DISP1", "N1", "D1", true);

            // --- ACT ---
            await _handler.Handle(command, CancellationToken.None);

            // --- ASSERT ---
            await _unitOfWork.ProductDetails.Received(1).AddAsync(Arg.Any<PromoProductDetail>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
