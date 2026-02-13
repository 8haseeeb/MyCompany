using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.CustomerRelations.Commands;


namespace Promotions.Application.CustomerRelations.Commands.Handlers
{
    public class DeleteCustomerRelationCommandHandler
        : IRequestHandler<DeleteCustomerRelationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerRelationCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            DeleteCustomerRelationCommand request,
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

            await _unitOfWork.CustomerRelations.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
