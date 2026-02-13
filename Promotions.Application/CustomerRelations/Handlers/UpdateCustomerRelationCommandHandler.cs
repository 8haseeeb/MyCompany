using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.CustomerRelations.Commands;


namespace Promotions.Application.CustomerRelations.Commands.Handlers
{
    public class UpdateCustomerRelationCommandHandler
        : IRequestHandler<UpdateCustomerRelationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerRelationCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            UpdateCustomerRelationCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.CustomerRelations.GetByIdAsync(
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart);

            if (entity == null)
                throw new Exception("Customer relation not found");

            entity.UpdateHierarchy(request.CodParentNode);
            entity.SetEndDate(request.DteEnd);

            await _unitOfWork.CustomerRelations.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
