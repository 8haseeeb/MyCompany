using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Application.CustomerRelations.Interfaces
{
    public interface ICustomerRelationRepository : Promotions.Domain.Shared.IRepository<CustomerRelation>
    {
        Task<CustomerRelation?> GetByIdAsync(
            string codHier,
            string codDiv,
            string codNode,
            int idLevel,
            DateTime dteStart);

        Task<List<CustomerRelation>> GetByNodeAndDivAsync(string codNode, string codDiv);
        Task<List<CustomerRelation>> GetByActionAsync(int idAction);
        Task<bool> ExistsAsync(string codHier, string codDiv, string codNode, int idLevel, DateTime dteStart);

        /// <summary>True if any customer relation row references this division.</summary>
        Task<bool> AnyWithCodDivAsync(string codDiv);
    }
}
