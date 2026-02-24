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

            if (action.LevParticipants == null || action.LevParticipants == 0)
                return new List<CustomerRelation>();

            // Strict Filter: 
            // 1. Join CustomerRelations with Participants to ensure we only get relations actually used in this action.
            // 2. Filter by the Promotion Header's Target Division (CodDiv) and Target Level (IdLevel).
            // 3. This resolves the user's issue where all participants/delivery points were showing up.
            
            var targetDiv = action.CodDiv?.Trim().ToUpper() ?? string.Empty;
            var targetLevel = action.LevParticipants ?? 0;

            return await _context.CustomerRelations
                .Join(_context.Participants,
                    cust => new { cust.CodHier, cust.CodDiv, cust.CodNode, cust.IdLevel, cust.DteStart },
                    part => new { part.CodHier, part.CodDiv, part.CodNode, part.IdLevel, part.DteStart },
                    (cust, part) => new { cust, part })
                .Where(x => x.part.IdAction == idAction)
                .Where(x => x.cust.CodDiv != null && x.cust.CodDiv.Trim().ToUpper() == targetDiv)
                .Where(x => x.cust.IdLevel == targetLevel)
                .Select(x => x.cust)
                .Distinct()
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
