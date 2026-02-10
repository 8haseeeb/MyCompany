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
        Task<int> GetMaxIdAsync();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Common.Interfaces.IAtomicTransaction> BeginTransactionAsync();
        Task AddMeasureFieldAsync(Promotions.Domain.Measures.PromoMeasureField measureField);
        Task<bool> ExistsMeasureFieldAsync(string codDiv, string codMeasure, string fieldName);
        Task<List<Promotions.Domain.Measures.PromoMeasureField>> GetMeasureFieldsByMeasureAsync(string codDiv, string codMeasure);
    }
}



