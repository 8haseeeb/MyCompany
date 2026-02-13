using MediatR;
using Promotions.Domain.DeliveryPoints;
using Promotions.Domain.CustomerRelations;
using Promotions.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.DeliveryPoints.Commands
{
    public class CreateDeliveryPointCommandHandler
        : IRequestHandler<CreateDeliveryPointCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeliveryPointCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            CreateDeliveryPointCommand request,
            CancellationToken cancellationToken)
        {
            // Auto-create CustomerRelation if it doesn't exist
            var exists = await _unitOfWork.CustomerRelations.ExistsAsync(
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart);

            if (!exists)
            {
                var newRelation = new CustomerRelation(
                    request.CodHier,
                    request.CodDiv,
                    request.CodNode,
                    request.IdLevel,
                    request.DteStart,
                    "ROOT"
                );
                await _unitOfWork.CustomerRelations.AddAsync(newRelation);
            }

            var entity = new PromoDeliveryPoint(
                request.IdAction,
                request.CodDeliveryPoint,
                request.FlgInclusion,
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart
            );

            await _unitOfWork.DeliveryPoints.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
