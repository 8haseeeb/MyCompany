using MediatR;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Application.CustomerRelations.Commands.Handlers
{
    public class CreateCustomerRelationCommandHandler
        : IRequestHandler<CreateCustomerRelationCommand, Unit>
    {
        private readonly ICustomerRelationRepository _repository;

        public CreateCustomerRelationCommandHandler(
            ICustomerRelationRepository repository)
        {
            _repository = repository;
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

            if (await _repository.ExistsAsync(request.CodHier, request.CodDiv, request.CodNode, request.IdLevel, request.DteStart))
            {
                return Unit.Value;
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
