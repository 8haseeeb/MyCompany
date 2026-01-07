using MediatR;
using Promotions.Application.CustomerRelations.Interfaces;

namespace Promotions.Application.CustomerRelations.Commands.Handlers
{
    public class DeleteCustomerRelationCommandHandler
        : IRequestHandler<DeleteCustomerRelationCommand, Unit>
    {
        private readonly ICustomerRelationRepository _repository;

        public DeleteCustomerRelationCommandHandler(
            ICustomerRelationRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            DeleteCustomerRelationCommand request,
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

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
