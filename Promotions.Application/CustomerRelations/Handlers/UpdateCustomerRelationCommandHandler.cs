using MediatR;
using Promotions.Application.CustomerRelations.Interfaces;

namespace Promotions.Application.CustomerRelations.Commands.Handlers
{
    public class UpdateCustomerRelationCommandHandler
        : IRequestHandler<UpdateCustomerRelationCommand, Unit>
    {
        private readonly ICustomerRelationRepository _repository;

        public UpdateCustomerRelationCommandHandler(
            ICustomerRelationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            UpdateCustomerRelationCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart);

            if (entity == null)
                throw new Exception("Customer relation not found");

            entity.CodParentNode = request.CodParentNode;
            entity.DteEnd = request.DteEnd;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
