using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Application.CustomerRelations.Interfaces
{
    public interface ICustomerRelationRepository
    {
        Task<CustomerRelation?> GetByIdAsync(
            string codHier,
            string codDiv,
            string codNode,
            int idLevel,
            DateTime dteStart);

        Task<List<CustomerRelation>> GetByNodeAndDivAsync(string codNode, string codDiv);

        Task<List<CustomerRelation>> GetAllAsync();
        Task AddAsync(CustomerRelation entity);
        Task UpdateAsync(CustomerRelation entity);
        Task DeleteAsync(CustomerRelation entity);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
