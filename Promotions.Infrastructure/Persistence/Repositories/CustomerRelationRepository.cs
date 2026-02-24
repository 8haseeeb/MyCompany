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

        public async Task<List<CustomerRelation>> GetByActionAsync(int idAction)
        {
            // We want to return only the "primary" customer relation for this promotion.
            // By convention, this is the relation associated with the first participant added.
            var firstParticipant = await _context.Participants
                .Where(p => p.IdAction == idAction)
                .OrderBy(p => p.CodParticipant) // Assuming the first one created has the lowest/specific code or order
                .FirstOrDefaultAsync();

            if (firstParticipant == null) return new List<CustomerRelation>();

            return await _context.CustomerRelations
                .Where(x => x.CodNode == firstParticipant.CodNode && x.CodDiv == firstParticipant.CodDiv)
                .ToListAsync();
        }

        // Standard CRUD & SaveChangesAsync are in base class
    }
}
