using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class CustomerRelationRepository : Repository<CustomerRelation>, ICustomerRelationRepository
    {
        public CustomerRelationRepository(PromotionsDbContext context) : base(context)
        {
        }

        public async Task<CustomerRelation?> GetByIdAsync(
            string codHier,
            string codDiv,
            string codNode,
            int idLevel,
            DateTime dteStart)
        {
            return await _context.CustomerRelations.FindAsync(
                codHier, codDiv, codNode, idLevel, dteStart);
        }

        public async Task<List<CustomerRelation>> GetByNodeAndDivAsync(string codNode, string codDiv)
        {
            return await _context.CustomerRelations
                .Where(x => x.CodNode == codNode && x.CodDiv == codDiv)
                .Include(x => x.Participants)
                .Include(x => x.DeliveryPoints)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string codHier, string codDiv, string codNode, int idLevel, DateTime dteStart)
        {
            return await _context.CustomerRelations.AnyAsync(x => 
                x.CodHier == codHier && 
                x.CodDiv == codDiv && 
                x.CodNode == codNode && 
                x.IdLevel == idLevel && 
                x.DteStart == dteStart);
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
