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
            var action = await _context.PromoActions
                .FirstOrDefaultAsync(a => a.IdAction == idAction);

            if (action == null) return new List<CustomerRelation>();

            // We filter relations that match the action's division and participant level
            // AND are actually associated with this action's participants.
            var query = _context.CustomerRelations
                .Where(x => x.CodDiv == action.CodDiv);

            if (action.LevParticipants.HasValue)
            {
                query = query.Where(x => x.IdLevel == action.LevParticipants.Value);
            }

            // Ensure we only return relations linked to this action
            return await query
                .Where(x => x.Participants.Any(p => p.IdAction == idAction))
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
