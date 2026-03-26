using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Promotions.Domain.PromoActions;

namespace Promotions.Application.PromoActions.Interfaces
{
    public interface IPromoActionRepository : Promotions.Domain.Shared.IRepository<PromoAction>
    {
        // Standard CRUD handled by IRepository

        Task<int> GetMaxIdAsync();

        /// <summary>True if a row exists in TA500PROMOACTION for this IdAction.</summary>
        Task<bool> ExistsIdActionAsync(int idAction);

        Task<Common.Interfaces.IAtomicTransaction> BeginTransactionAsync();
        Task AddMeasureFieldAsync(Promotions.Domain.Measures.PromoMeasureField measureField);
        Task<bool> ExistsMeasureFieldAsync(string codDiv, string codMeasure, string fieldName);
        Task<List<Promotions.Domain.Measures.PromoMeasureField>> GetMeasureFieldsByMeasureAsync(string codDiv, string codMeasure);
    }
}



