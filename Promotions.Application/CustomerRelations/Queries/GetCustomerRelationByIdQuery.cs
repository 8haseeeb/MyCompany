using MediatR;
using Promotions.Application.CustomerRelations.Dtos;

namespace Promotions.Application.CustomerRelations.Queries
{
    public class GetCustomerRelationByIdQuery
        : IRequest<CustomerRelationDto>
    {
        public string CodHier { get; }
        public string CodDiv { get; }
        public string CodNode { get; }
        public int IdLevel { get; }
        public DateTime DteStart { get; }

        public GetCustomerRelationByIdQuery(
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
