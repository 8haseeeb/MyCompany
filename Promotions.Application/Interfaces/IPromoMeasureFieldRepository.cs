using Promotions.Domain.Measures;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Interfaces
{
    public interface IPromoMeasureFieldRepository
    {
        Task AddAsync(PromoMeasureField entity, CancellationToken cancellationToken);
        Task<PromoMeasureField?> GetByIdAsync(string codDiv, string codMeasure, string fieldName, CancellationToken cancellationToken);
        Task UpdateAsync(PromoMeasureField entity, CancellationToken cancellationToken);
     
        Task DeleteAsync(PromoMeasureField entity, CancellationToken cancellationToken);
        Task<List<PromoMeasureField>> GetByMeasureAsync(string codMeasure, CancellationToken cancellationToken);

        Task<List<PromoMeasureField>> GetAllAsync(CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }

}
