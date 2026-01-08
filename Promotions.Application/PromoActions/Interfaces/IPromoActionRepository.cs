using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promotions.Domain.PromoActions;

namespace Promotions.Application.PromoActions.Interfaces
{
    public interface IPromoActionRepository
    {
        Task AddAsync(PromoAction action);
        Task UpdateAsync(PromoAction action);
        Task DeleteAsync(PromoAction action);

        Task<PromoAction?> GetByIdAsync(int idAction);
        Task<List<PromoAction>> GetAllAsync();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Common.Interfaces.IAtomicTransaction> BeginTransactionAsync();


    }
}
