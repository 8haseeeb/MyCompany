using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Participant.Interfaces;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Application.Interfaces;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Infrastructure.Persistence.Repositories;

namespace Promotions.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PromotionsDbContext _context;

        public UnitOfWork(
            PromotionsDbContext context,
            IPromoActionRepository promoActions,
            IProductRepository products,
            IParticipantRepository participants,
            IDeliveryPointRepository deliveryPoints,
            IProductDetailRepository productDetails,
            IPromoArticleRepository promoArticles,
            IPromoMeasureFieldRepository measureFields,
            ICustomerRelationRepository customerRelations)
        {
            _context = context;
            PromoActions = promoActions;
            Products = products;
            Participants = participants;
            DeliveryPoints = deliveryPoints;
            ProductDetails = productDetails;
            PromoArticles = promoArticles;
            MeasureFields = measureFields;
            CustomerRelations = customerRelations;
        }

        public IPromoActionRepository PromoActions { get; }
        public IProductRepository Products { get; }
        public IParticipantRepository Participants { get; }
        public IDeliveryPointRepository DeliveryPoints { get; }
        public IProductDetailRepository ProductDetails { get; }
        public IPromoArticleRepository PromoArticles { get; }
        public IPromoMeasureFieldRepository MeasureFields { get; }
        public ICustomerRelationRepository CustomerRelations { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
