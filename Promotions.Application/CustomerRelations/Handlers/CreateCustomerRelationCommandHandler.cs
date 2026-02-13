using MediatR;
using Promotions.Domain.CustomerRelations;
using Promotions.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.CustomerRelations.Commands.Handlers
{
    public class CreateCustomerRelationCommandHandler
        : IRequestHandler<CreateCustomerRelationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerRelationCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            CreateCustomerRelationCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new CustomerRelation(
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart,
                request.CodParentNode
            );
            entity.SetEndDate(request.DteEnd);

            if (await _unitOfWork.CustomerRelations.ExistsAsync(request.CodHier, request.CodDiv, request.CodNode, request.IdLevel, request.DteStart))
            {
                return Unit.Value;
            }

            await _unitOfWork.CustomerRelations.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
