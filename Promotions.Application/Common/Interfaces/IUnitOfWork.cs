using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Participant.Interfaces;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Application.Interfaces;
using Promotions.Application.CustomerRelations.Interfaces;

namespace Promotions.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IPromoActionRepository PromoActions { get; }
        IProductRepository Products { get; }
        IParticipantRepository Participants { get; }
        IDeliveryPointRepository DeliveryPoints { get; }
        IProductDetailRepository ProductDetails { get; }
        IPromoArticleRepository PromoArticles { get; }
        IPromoMeasureFieldRepository MeasureFields { get; }
        ICustomerRelationRepository CustomerRelations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
