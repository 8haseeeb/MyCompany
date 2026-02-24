using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromotionDetails.Handlers
{
    using global::Promotions.Application.PromotionDetails.Queries;
    using global::Promotions.Application.PromotionDetails.Dtos;
    using global::Promotions.Application.PromoActions.Interfaces;
    using global::Promotions.Application.Products.Interfaces;
    using global::Promotions.Application.ProductDetails.Interfaces;
    using global::Promotions.Application.Participant.Interfaces;
    using global::Promotions.Application.DeliveryPoints.Interfaces;
    using global::Promotions.Application.CustomerRelations.Interfaces;
    using global::Promotions.Application.Interfaces;
    using global::Promotions.Application.Products.Dtos;
    using global::Promotions.Application.ProductDetails.Dtos;
    using global::Promotions.Application.Measures.Dtos;
    using global::Promotions.Application.Participants.Dtos;
    using global::Promotions.Application.DeliveryPoints.Dtos;
    using global::Promotions.Application.CustomerRelations.Dtos;
    using global::Promotions.Application.PromoArticles.Dtos;
    using global::Promotions.Application.PromoArticles.Interfaces;
    using global::Promotions.Domain.PromoActions;
    using global::Promotions.Domain.Products;
    using global::Promotions.Domain.Measures;

    public class GetCompletePromotionQueryHandler : IRequestHandler<GetCompletePromotionQuery, CompletePromotionDto>
    {
        private readonly IPromoActionRepository _promoActionRepo;
        private readonly IProductRepository _productRepo;
        private readonly IProductDetailRepository _productDetailRepo;
        private readonly IParticipantRepository _participantRepo;
        private readonly IDeliveryPointRepository _deliveryPointRepo;
        private readonly ICustomerRelationRepository _customerRepo;
        private readonly IPromoArticleRepository _articleRepo;
        private readonly IPromoMeasureFieldRepository _measureFieldRepo;

        public GetCompletePromotionQueryHandler(
            IPromoActionRepository promoActionRepo,
            IProductRepository productRepo,
            IProductDetailRepository productDetailRepo,
            IParticipantRepository participantRepo,
            IDeliveryPointRepository deliveryPointRepo,
            ICustomerRelationRepository customerRepo,
            IPromoArticleRepository articleRepo,
            IPromoMeasureFieldRepository measureFieldRepo)
        {
            _promoActionRepo = promoActionRepo;
            _productRepo = productRepo;
            _productDetailRepo = productDetailRepo;
            _participantRepo = participantRepo;
            _deliveryPointRepo = deliveryPointRepo;
            _customerRepo = customerRepo;
            _articleRepo = articleRepo;
            _measureFieldRepo = measureFieldRepo;
        }

        public async Task<CompletePromotionDto> Handle(GetCompletePromotionQuery request, CancellationToken cancellationToken)
        {
            var idAction = request.IdAction;

            // Fetch data sequentially to avoid concurrent DbContext usage
            var actionEntity = await _promoActionRepo.GetByIdAsync(idAction);
            var productEntities = await _productRepo.GetByActionAsync(idAction);
            var productDetailEntitiesFromRepo = await _productDetailRepo.GetByActionAsync(idAction);
            
            // Collect all details (repo + those potentially loaded with products)
            var detailsFromProducts = productEntities.SelectMany(p => p.Details).ToList();
            var productDetailEntities = productDetailEntitiesFromRepo
                .Concat(detailsFromProducts)
                .GroupBy(d => new { d.IdAction, d.CodProduct, d.LevProduct, d.CodDisplay, d.CodNode, d.CodDiv })
                .Select(g => g.First())
                .ToList();

            var participants = await _participantRepo.GetByActionAsync(idAction);
            var deliveryPoints = await _deliveryPointRepo.GetByActionAsync(idAction);
            var allMeasureFields = await _measureFieldRepo.GetAllAsync();
            
            // Article Fetching Logic: Since IdAction is ignored in TA5150PROMOARTICLES,
            // we must fetch articles that match the CodDiv/CodNode present in the promotion's ProductDetails.
            var articleNodes = productDetailEntities
                .Select(d => (d.CodDiv, d.CodNode))
                .Distinct()
                .ToList();

            var actionSpecificArticles = await _articleRepo.GetByNodesAsync(articleNodes);

            // Map ProductDetails to DTOs
            var productDetailDtos = productDetailEntities.Select(d => new ProductDetailDto
            {
                IdAction = d.IdAction,
                CodProduct = d.CodProduct,
                LevProduct = d.LevProduct,
                CodDisplay = d.CodDisplay,
                CodNode = d.CodNode,
                CodDiv = d.CodDiv,
                FlgInclusion = d.FlgInclusion,
                Articles = new List<PromoArticleDto>() // Will be populated below if matching
            }).ToList();

            // Build a lookup of details grouped by (IdAction, CodProduct, LevProduct, CodDisplay) for product mapping
            var detailsByProduct = productDetailEntities
                .GroupBy(d => new { d.IdAction, d.CodProduct, d.LevProduct, d.CodDisplay })
                .ToDictionary(g => g.Key, g => g.ToList());

            // Map Products to DTOs (attach their details)
            var productDtos = productEntities.Select(p =>
            {
                var key = new { p.IdAction, p.CodProduct, p.LevProduct, p.CodDisplay };
                var details = detailsByProduct.ContainsKey(key) ? detailsByProduct[key] : new List<global::Promotions.Domain.ProductDetails.PromoProductDetail>();

                return new ProductDto
                {
                    IdAction = p.IdAction,
                    CodProduct = p.CodProduct,
                    LevProduct = p.LevProduct,
                    CodDisplay = p.CodDisplay,
                    CodDiv = p.CodDiv,
                    QtyEstimated = p.QtyEstimated,
                    PerceDiscount1 = p.PerceDiscount1,
                    PerceDiscount2 = p.PerceDiscount2,
                    NumMeasure = p.NumMeasure,
                    CodMeasure = p.CodMeasure,
                    Details = details.Select(d => new ProductDetailDto
                    {
                        IdAction = d.IdAction,
                        CodProduct = d.CodProduct,
                        LevProduct = d.LevProduct,
                        CodDisplay = d.CodDisplay,
                        CodNode = d.CodNode,
                        CodDiv = d.CodDiv,
                        FlgInclusion = d.FlgInclusion,
                        Articles = actionSpecificArticles
                            .Where(a => a.CodDiv == d.CodDiv && a.CodNode == d.CodNode)
                            .Select(a => new PromoArticleDto
                            {
                                CodDiv = a.CodDiv ?? string.Empty,
                                CodNode = a.CodNode ?? string.Empty,
                                CodProduct = a.CodProduct ?? string.Empty,
                                LevProduct = a.LevProduct,
                                CodDisplay = a.CodDisplay ?? string.Empty,
                                CodNode1 = a.CodNode1,
                                CodNode2 = a.CodNode2,
                                CodNodeN = a.CodNodeN,
                                IdAction = idAction
                            }).ToList()
                    }).ToList(),
                    // Filter measure fields that match this product's division and measure code
                    MeasureFields = allMeasureFields
                        .Where(m => m.CodDiv == p.CodDiv && m.CodMeasure == p.CodMeasure)
                        .Select(m => new global::Promotions.Application.Measures.Dtos.PromoMeasureFieldDto
                        {
                            CodDiv = m.CodDiv,
                            CodMeasure = m.CodMeasure,
                            FieldName = m.FieldName,
                            Formula = m.Formula
                        }).ToList()
                };
            }).ToList();

            // Populate Articles list for the CompletePromotionDto
            var promoArticles = actionSpecificArticles.Select(a => new PromoArticleDto
            {
                CodProduct = a.CodProduct,
                LevProduct = a.LevProduct,
                CodDisplay = a.CodDisplay,
                CodDiv = a.CodDiv ?? string.Empty,
                CodNode = a.CodNode ?? string.Empty,
                CodNode1 = a.CodNode1,
                CodNode2 = a.CodNode2,
                CodNodeN = a.CodNodeN,
                IdAction = idAction
            }).ToList();
            
            // If repository articles are empty, fallback to derived articles from details
            if (!promoArticles.Any())
            {
                promoArticles = productDetailEntities
                    .GroupBy(d => new { d.CodDiv, d.CodNode })
                    .Select(g => new PromoArticleDto
                    {
                        IdAction = idAction,
                        CodDiv = g.Key.CodDiv,
                        CodNode = g.Key.CodNode,
                        CodProduct = g.First().CodProduct,
                        LevProduct = g.First().LevProduct,
                        CodDisplay = g.First().CodDisplay
                    }).ToList();
            }

            var promoMeasureFields = productDtos
                .SelectMany(p => p.MeasureFields)
                .DistinctBy(m => new { m.CodDiv, m.CodMeasure, m.FieldName })
                .ToList();

            // Customer Resolution: Filtered by action via the new repository method
            var customers = await _customerRepo.GetByActionAsync(idAction);
            var customerDtos = customers.Select(customer => new CustomerRelationDto
            {
                IdAction = idAction,
                CodHier = customer.CodHier ?? string.Empty,
                CodDiv = customer.CodDiv ?? string.Empty,
                CodNode = customer.CodNode ?? string.Empty,
                IdLevel = customer.IdLevel,
                DteStart = customer.DteStart,
                CodParentNode = customer.CodParentNode ?? string.Empty,
                DteEnd = customer.DteEnd
            }).ToList();

            return new CompletePromotionDto
            {
                PromoAction = actionEntity != null ? new global::Promotions.Application.PromoActions.Dtos.PromoActionDto
                {
                    IdAction = actionEntity.IdAction,
                    Name = actionEntity.Name,
                    CodDiv = actionEntity.CodDiv,
                    DteStartSellIn = actionEntity.DteStartSellIn,
                    DteEndSellIn = actionEntity.DteEndSellIn,
                    DteStartSellOut = actionEntity.DteStartSellOut,
                    DteEndSellOut = actionEntity.DteEndSellOut,
                    DocumentKey = actionEntity.DocumentKey,
                    DteToShost = actionEntity.DteToShost,
                    LevParticipants = actionEntity.LevParticipants
                } : null,
                Products = productDtos,
                ProductDetails = productDetailDtos,
                Articles = promoArticles,
                MeasureFields = promoMeasureFields,
                Participants = participants.Select(p => new ParticipantDto
                {
                    IdAction = p.IdAction,
                    CodParticipant = p.CodParticipant,
                    CodHier = p.CodHier,
                    CodDiv = p.CodDiv,
                    CodNode = p.CodNode,
                    IdLevel = p.IdLevel,
                    FlgInclusion = p.FlgInclusion
                }).ToList(),
                DeliveryPoints = deliveryPoints.Select(d => new DeliveryPointDto
                {
                    IdAction = d.IdAction,
                    CodDeliveryPoint = d.CodDeliveryPoint,
                    CodHier = d.CodHier,
                    CodDiv = d.CodDiv,
                    CodNode = d.CodNode,
                    IdLevel = d.IdLevel,
                    FlgInclusion = d.FlgInclusion
                }).ToList(),
                Customers = customerDtos
            };
        }
    }
}
