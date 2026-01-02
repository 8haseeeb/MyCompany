using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Infrastructure.Persistence.Repositories
{
    public class CustomerRelationRepository : ICustomerRelationRepository
    {
        private readonly PromotionsDbContext _context;

        public CustomerRelationRepository(PromotionsDbContext context)
        {
            _context = context;
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

        public async Task<List<CustomerRelation>> GetAllAsync()
        {
            return await _context.CustomerRelations.ToListAsync();
        }

        public async Task AddAsync(CustomerRelation entity)
        {
            _context.CustomerRelations.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomerRelation entity)
        {
            _context.CustomerRelations.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CustomerRelation entity)
        {
            _context.CustomerRelations.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
