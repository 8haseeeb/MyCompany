using Promotions.Domain.Measures;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Interfaces
{
    public interface IPromoMeasureFieldRepository : Promotions.Domain.Shared.IRepository<PromoMeasureField>
    {
        Task<PromoMeasureField?> GetByIdAsync(string codDiv, string codMeasure, string fieldName, CancellationToken cancellationToken);
        Task<List<PromoMeasureField>> GetByMeasureAsync(string codMeasure, CancellationToken cancellationToken);
    }

}
