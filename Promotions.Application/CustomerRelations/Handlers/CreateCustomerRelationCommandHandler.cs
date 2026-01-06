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
            var entity = new CustomerRelation
            {
                CodHier = request.CodHier,
                CodDiv = request.CodDiv,
                CodNode = request.CodNode,
                IdLevel = request.IdLevel,
                DteStart = request.DteStart,
                CodParentNode = request.CodParentNode,
                DteEnd = request.DteEnd
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
