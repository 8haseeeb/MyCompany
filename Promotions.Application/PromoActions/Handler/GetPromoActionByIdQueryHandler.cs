using MediatR;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.PromoActions.Queries;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.Participants.Dtos;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.Measures.Dtos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Handler
{
    public class GetPromoActionByIdQueryHandler : IRequestHandler<GetPromoActionByIdQuery, PromoActionDetailDto?>
    {
        private readonly IPromoActionRepository _repository;

        public GetPromoActionByIdQueryHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<PromoActionDetailDto?> Handle(GetPromoActionByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.IdAction);

            if (entity == null) return null;

            return new PromoActionDetailDto
            {
                IdAction = entity.IdAction,
                Name = entity.Name,
                CodDiv = entity.CodDiv,
                CodContractor = entity.CodContractor,
                DteStartSellIn = entity.DteStartSellIn,
                DteEndSellIn = entity.DteEndSellIn,
                DteStartSellOut = entity.DteStartSellOut,
                DteEndSellOut = entity.DteEndSellOut,
                DocumentKey = entity.DocumentKey,
                DteToShost = entity.DteToShost,
                LevParticipants = entity.LevParticipants,
                
                CustomerRelation = entity.Contractor == null ? null : new CustomerRelationDetailDto
                {
                    CodHier = entity.Contractor.CodHier,
                    CodDiv = entity.Contractor.CodDiv,
                    CodNode = entity.Contractor.CodNode,
                    IdLevel = entity.Contractor.IdLevel,
                    DteStart = entity.Contractor.DteStart,
                    CodParentNode = entity.Contractor.CodParentNode,
                    DteEnd = entity.Contractor.DteEnd,
                    Participants = entity.Contractor.Participants.Select(p => new ParticipantDto
                    {
                        IdAction = p.IdAction,
                        CodParticipant = p.CodParticipant,
                        FlgInclusion = p.FlgInclusion,
                        CodHier = p.CodHier,
                        CodDiv = p.CodDiv,
                        CodNode = p.CodNode,
                        IdLevel = p.IdLevel,
                        DteStart = p.DteStart
                    }).ToList(),
                    DeliveryPoints = entity.Contractor.DeliveryPoints.Select(dp => new DeliveryPointDto
                    {
                        IdAction = dp.IdAction,
                        CodDeliveryPoint = dp.CodDeliveryPoint,
                        FlgInclusion = dp.FlgInclusion,
                        CodHier = dp.CodHier,
                        CodDiv = dp.CodDiv,
                        CodNode = dp.CodNode,
                        IdLevel = dp.IdLevel,
                        DteStart = dp.DteStart
                    }).ToList()
                },

                Products = entity.Products.Select(p => new PromoProductDetailViewDto
                {
                    CodProduct = p.CodProduct,
                    LevProduct = p.LevProduct,
                    CodDisplay = p.CodDisplay,
                    CodDiv = p.CodDiv,
                    QtyEstimated = p.QtyEstimated,
                    PerceDiscount1 = p.PerceDiscount1,
                    PerceDiscount2 = p.PerceDiscount2,
                    NumMeasure = p.NumMeasure,
                    CodMeasure = p.CodMeasure,
                    MeasureFields = p.MeasureFields.Select(mf => new PromoMeasureFieldDto
                    {
                        IdAction = mf.IdAction,
                        CodProduct = mf.CodProduct,
                        LevProduct = mf.LevProduct,
                        CodDisplay = mf.CodDisplay,
                        CodDiv = mf.CodDiv,
                        CodMeasure = mf.CodMeasure,
                        FieldName = mf.FieldName,
                        Formula = mf.Formula
                    }).ToList(),
                    Details = p.Details.Select(d => new ProductDetailHierarchyDto
                    {
                        CodNode = d.CodNode,
                        FlgInclusion = d.FlgInclusion,
                        Articles = d.Articles.Select(a => new PromoArticleDto
                        {
                            IdAction = a.IdAction,
                            CodProduct = a.CodProduct,
                            LevProduct = a.LevProduct,
                            CodDisplay = a.CodDisplay,
                            CodDiv = a.CodDiv,
                            CodNode = a.CodNode,
                            CodNode1 = a.CodNode1,
                            CodNode2 = a.CodNode2,
                            CodNodeN = a.CodNodeN
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }
    }
}
