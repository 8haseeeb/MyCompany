using MediatR;

namespace Promotions.Application.CustomerRelations.Commands
{
    public class DeleteCustomerRelationCommand : IRequest<Unit>
    {
        public string CodHier { get; }
        public string CodDiv { get; }
        public string CodNode { get; }
        public int IdLevel { get; }
        public DateTime DteStart { get; }

        public DeleteCustomerRelationCommand(
            string codHier,
            string codDiv,
            string codNode,
            int idLevel,
            DateTime dteStart)
        {
            CodHier = codHier;
            CodDiv = codDiv;
            CodNode = codNode;
            IdLevel = idLevel;
            DteStart = dteStart;
        }
    }
}
