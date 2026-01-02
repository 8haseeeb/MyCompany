using MediatR;

namespace Promotions.Application.CustomerRelations.Commands
{
    public class CreateCustomerRelationCommand : IRequest<Unit>
    {
        public string CodHier { get; }
        public string CodDiv { get; }
        public string CodNode { get; }
        public int IdLevel { get; }
        public DateTime DteStart { get; }
        public string CodParentNode { get; }
        public DateTime? DteEnd { get; }

        public CreateCustomerRelationCommand(
            string codHier,
            string codDiv,
            string codNode,
            int idLevel,
            DateTime dteStart,
            string codParentNode,
            DateTime? dteEnd)
        {
            CodHier = codHier;
            CodDiv = codDiv;
            CodNode = codNode;
            IdLevel = idLevel;
            DteStart = dteStart;
            CodParentNode = codParentNode;
            DteEnd = dteEnd;
        }
    }
}
