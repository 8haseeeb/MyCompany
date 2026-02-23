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
    using global::Promotions.Application.PromoArticles.Interfaces;
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
    using global::Promotions.Domain.PromoActions;
    using global::Promotions.Domain.Products;
    using global::Promotions.Domain.Measures;

    public class GetCompletePromotionQueryHandler : IRequestHandler<GetCompletePromotionQuery, CompletePromotionDto>
    {
        private readonly IPromoActionRepository _promoActionRepo;
        private readonly IProductRepository _productRepo;
        private readonly IParticipantRepository _participantRepo;
        private readonly IDeliveryPointRepository _deliveryPointRepo;
        private readonly ICustomerRelationRepository _customerRepo;
        private readonly IPromoMeasureFieldRepository _measureFieldRepo;

        public GetCompletePromotionQueryHandler(
            IPromoActionRepository promoActionRepo,
            IProductRepository productRepo,
            IParticipantRepository participantRepo,
            IDeliveryPointRepository deliveryPointRepo,
            ICustomerRelationRepository customerRepo,
            IPromoMeasureFieldRepository measureFieldRepo)
        {
            _promoActionRepo = promoActionRepo;
            _productRepo = productRepo;
            _participantRepo = participantRepo;
            _deliveryPointRepo = deliveryPointRepo;
            _customerRepo = customerRepo;
            _measureFieldRepo = measureFieldRepo;
        }

        public async Task<CompletePromotionDto> Handle(GetCompletePromotionQuery request, CancellationToken cancellationToken)
        {
            var idAction = request.IdAction;

            // Fetch data sequentially to avoid concurrent DbContext usage
            var actionEntity = await _promoActionRepo.GetByIdAsync(idAction);
            var productEntities = await _productRepo.GetByActionAsync(idAction);
            var participants = await _participantRepo.GetByActionAsync(idAction);
            var deliveryPoints = await _deliveryPointRepo.GetByActionAsync(idAction);
            var allCustomers = await _customerRepo.GetAllAsync();
            var allMeasureFields = await _measureFieldRepo.GetAllAsync();

            // Map Products to DTOs
            var productDtos = productEntities.Select(p => new ProductDto
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
                Details = p.Details.Select(d => new ProductDetailDto
                {
                    IdAction = d.IdAction,
                    CodProduct = d.CodProduct,
                    LevProduct = d.LevProduct,
                    CodDisplay = d.CodDisplay,
                    CodNode = d.CodNode,
                    CodDiv = d.CodDiv,
                    FlgInclusion = d.FlgInclusion,
                    Articles = d.Article != null ? new List<global::Promotions.Application.PromoArticles.Dtos.PromoArticleDto>
                    {
                        new global::Promotions.Application.PromoArticles.Dtos.PromoArticleDto
                        {
                            IdAction = d.Article.IdAction,
                            CodProduct = d.Article.CodProduct,
                            LevProduct = d.Article.LevProduct,
                            CodDisplay = d.Article.CodDisplay,
                            CodDiv = d.Article.CodDiv,
                            CodNode = d.Article.CodNode,
                            CodNode1 = d.Article.CodNode1,
                            CodNode2 = d.Article.CodNode2,
                            CodNodeN = d.Article.CodNodeN
                        }
                    } : new List<global::Promotions.Application.PromoArticles.Dtos.PromoArticleDto>()
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
            }).ToList();

            // Flatten articles, product details, and all measure fields used in this promotion for the root tabs
            var promoArticles = productDtos.SelectMany(p => p.Details.SelectMany(d => d.Articles)).ToList();
            var productDetails = productDtos.SelectMany(p => p.Details).ToList();
            var promoMeasureFields = productDtos.SelectMany(p => p.MeasureFields).DistinctBy(m => new { m.CodDiv, m.CodMeasure, m.FieldName }).ToList();

            // Refined Customer Filtering: Only include customers linked to this promotion's participants or delivery points
            var promoNodes = participants.Select(p => new { p.CodHier, p.CodNode, p.IdLevel })
                .Concat(deliveryPoints.Select(d => new { d.CodHier, d.CodNode, d.IdLevel }))
                .Distinct()
                .ToList();

            var filteredCustomers = allCustomers
                .Where(c => promoNodes.Any(node => node.CodHier == c.CodHier && node.CodNode == c.CodNode && node.IdLevel == c.IdLevel))
                .Select(c => new CustomerRelationDto
                {
                    IdAction = idAction, // Assign the searched IdAction
                    CodHier = c.CodHier,
                    CodDiv = c.CodDiv,
                    CodNode = c.CodNode,
                    IdLevel = c.IdLevel,
                    DteStart = c.DteStart,
                    CodParentNode = c.CodParentNode,
                    DteEnd = c.DteEnd
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
                ProductDetails = productDetails,
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
                Customers = filteredCustomers
            };
        }
    }
}
