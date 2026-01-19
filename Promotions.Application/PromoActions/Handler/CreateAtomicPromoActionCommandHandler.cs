using MediatR;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Participants;
using Promotions.Domain.Products;
using Promotions.Domain.DeliveryPoints;
using Promotions.Domain.ProductDetails;
using Promotions.Domain.Articles;
using Promotions.Domain.Measures;

using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class CreateAtomicPromoActionCommandHandler : IRequestHandler<CreateAtomicPromoActionCommand, Unit>
    {
        private readonly IPromoActionRepository _repository;
        private readonly CustomerRelations.Interfaces.ICustomerRelationRepository _relationRepository;
        private readonly Participant.Interfaces.IParticipantRepository _participantRepository;
        private readonly DeliveryPoints.Interfaces.IDeliveryPointRepository _dpRepository;

        public CreateAtomicPromoActionCommandHandler(
            IPromoActionRepository repository,
            CustomerRelations.Interfaces.ICustomerRelationRepository relationRepository,
            Participant.Interfaces.IParticipantRepository participantRepository,
            DeliveryPoints.Interfaces.IDeliveryPointRepository dpRepository)
        {
            _repository = repository;
            _relationRepository = relationRepository;
            _participantRepository = participantRepository;
            _dpRepository = dpRepository;
        }

        public async Task<Unit> Handle(CreateAtomicPromoActionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var allRelations = await _relationRepository.GetAllAsync();
            var allParticipants = await _participantRepository.GetAllAsync();
            var allDps = await _dpRepository.GetAllAsync();

            // Resolve missing hierarchy data for Participants
            foreach (var p in dto.Participants)
            {
                if (string.IsNullOrEmpty(p.CodHier) || string.IsNullOrEmpty(p.CodNode))
                {
                    // 1. Try matching against CustomerRelation table directly (Node or Hier)
                    var match = allRelations.FirstOrDefault(r => 
                        r.CodNode.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase) || 
                        r.CodHier.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase));
                    
                    if (match == null)
                    {
                        // 2. Try matching against existing participants list if user provided a specific participant code
                        var pMatch = allParticipants.FirstOrDefault(ap => ap.CodParticipant.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase));
                        if (pMatch != null)
                        {
                            p.CodHier = pMatch.CodHier;
                            p.CodDiv = pMatch.CodDiv;
                            p.CodNode = pMatch.CodNode;
                            p.IdLevel = pMatch.IdLevel;
                            p.DteStart = pMatch.DteStart;
                            continue; // Resolved via existing participant
                        }

                        // 3. Smart Suffix Resolution (e.g., P-2323 matches C-2323 via "2323")
                        var suffix = p.CodParticipant.Contains("-") ? p.CodParticipant.Split('-').Last() : p.CodParticipant;
                        match = allRelations.FirstOrDefault(r => 
                            r.CodNode.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) || 
                            r.CodNode.Equals(suffix, StringComparison.OrdinalIgnoreCase) ||
                            r.CodHier.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) ||
                            r.CodHier.Equals(suffix, StringComparison.OrdinalIgnoreCase));
                    }

                    if (match != null)
                    {
                        p.CodHier = match.CodHier;
                        p.CodDiv = match.CodDiv;
                        p.CodNode = match.CodNode;
                        p.IdLevel = match.IdLevel;
                        p.DteStart = match.DteStart;
                    }
                    else
                    {
                        throw new System.Collections.Generic.KeyNotFoundException($"Participant hierarchy could not be resolved for code: {p.CodParticipant}. Please ensure a Customer Relation or an existing Participant record exists for this code.");
                    }
                }
            }

            // Resolve missing hierarchy data for Delivery Points
            foreach (var dp in dto.DeliveryPoints)
            {
                if (string.IsNullOrEmpty(dp.CodHier) || string.IsNullOrEmpty(dp.CodNode))
                {
                    // 1. Try matching against CustomerRelation table directly
                    var match = allRelations.FirstOrDefault(r => 
                        r.CodNode.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase) || 
                        r.CodHier.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase));
                    
                    if (match == null)
                    {
                        // 2. Try matching against existing delivery points list
                        var dpMatch = allDps.FirstOrDefault(adp => adp.CodDeliveryPoint.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase));
                        if (dpMatch != null)
                        {
                            dp.CodHier = dpMatch.CodHier;
                            dp.CodDiv = dpMatch.CodDiv;
                            dp.CodNode = dpMatch.CodNode;
                            dp.IdLevel = dpMatch.IdLevel;
                            dp.DteStart = dpMatch.DteStart;
                            continue; // Resolved
                        }

                        // 3. Smart Suffix Resolution
                        var suffix = dp.CodDeliveryPoint.Contains("-") ? dp.CodDeliveryPoint.Split('-').Last() : dp.CodDeliveryPoint;
                        match = allRelations.FirstOrDefault(r => 
                            r.CodNode.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) || 
                            r.CodNode.Equals(suffix, StringComparison.OrdinalIgnoreCase) ||
                            r.CodHier.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) ||
                            r.CodHier.Equals(suffix, StringComparison.OrdinalIgnoreCase));
                    }

                    if (match != null)
                    {
                        dp.CodHier = match.CodHier;
                        dp.CodDiv = match.CodDiv;
                        dp.CodNode = match.CodNode;
                        dp.IdLevel = match.IdLevel;
                        dp.DteStart = match.DteStart;
                    }
                    else
                    {
                        throw new System.Collections.Generic.KeyNotFoundException($"Delivery Point hierarchy could not be resolved for code: {dp.CodDeliveryPoint}. Please ensure a Customer Relation or an existing Delivery Point record exists for this code.");
                    }
                }
            }

            var action = new PromoAction
            {
                IdAction = dto.IdAction,
                Name = dto.Name,
                CodDiv = dto.CodDiv,
                DteStartSellIn = dto.DteStartSellIn,
                DteEndSellIn = dto.DteEndSellIn,
                DteStartSellOut = dto.DteStartSellOut,
                DteEndSellOut = dto.DteEndSellOut,
                DocumentKey = dto.DocumentKey,
                DteToShost = dto.DteToShost,
                LevParticipants = dto.LevParticipants,

                // Map Participants
                Participants = dto.Participants.Select(p => new PromoParticipants
                {
                    IdAction = dto.IdAction,
                    CodParticipant = p.CodParticipant,
                    FlgInclusion = p.FlgInclusion,
                    CodHier = p.CodHier ?? string.Empty,
                    CodDiv = p.CodDiv ?? string.Empty,
                    CodNode = p.CodNode ?? string.Empty,
                    IdLevel = p.IdLevel ?? 0,
                    DteStart = p.DteStart ?? DateTime.MinValue
                }).ToList(),

                // Map Delivery Points
                DeliveryPoints = dto.DeliveryPoints.Select(dp => new PromoDeliveryPoint
                {
                    IdAction = dto.IdAction,
                    CodDeliveryPoint = dp.CodDeliveryPoint,
                    FlgInclusion = dp.FlgInclusion,
                    CodHier = dp.CodHier ?? string.Empty,
                    CodDiv = dp.CodDiv ?? string.Empty,
                    CodNode = dp.CodNode ?? string.Empty,
                    IdLevel = dp.IdLevel ?? 0,
                    DteStart = dp.DteStart ?? DateTime.MinValue
                }).ToList(),

                // Map Products (Hierarchical)
                Products = dto.Products.Select(p => new PromoProduct
                {
                    IdAction = dto.IdAction,
                    CodProduct = p.CodProduct ?? string.Empty,
                    LevProduct = p.LevProduct ?? 0,
                    CodDisplay = p.CodDisplay ?? string.Empty,
                    CodDiv = p.CodDiv,
                    QtyEstimated = p.QtyEstimated,
                    PerceDiscount1 = p.PerceDiscount1,
                    PerceDiscount2 = p.PerceDiscount2,
                    NumMeasure = p.NumMeasure,
                    CodMeasure = p.CodMeasure,


                    // Map Nested Details
                    Details = p.Details.Select(d => new PromoProductDetail
                    {
                        IdAction = dto.IdAction,
                        CodProduct = p.CodProduct ?? string.Empty,
                        LevProduct = p.LevProduct ?? 0,
                        CodDisplay = p.CodDisplay ?? string.Empty,

                        CodNode = d.CodNode,
                        CodDiv = d.CodDiv,
                        FlgInclusion = d.FlgInclusion,

                        // Map Nested Articles
                        Articles = d.Articles.Select(a => new PromoArticle
                        {
                            IdAction = dto.IdAction,
                            CodProduct = p.CodProduct ?? string.Empty,
                            LevProduct = p.LevProduct ?? 0,
                            CodDisplay = p.CodDisplay ?? string.Empty,
                            CodDiv = a.CodDiv,

                            CodNode = a.CodNode,
                            CodNode1 = a.CodNode1,
                            CodNode2 = a.CodNode2,
                            CodNodeN = a.CodNodeN
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            // Collect and Map Master Measure Fields (Unique by Div, Measure, and FieldName)
            var measureFields = dto.Products
                .Where(p => p.MeasureFields != null && p.MeasureFields.Any())
                .SelectMany(p => p.MeasureFields.Select(mf => new PromoMeasureField
                {
                    CodDiv = p.CodDiv,
                    CodMeasure = p.CodMeasure ?? string.Empty,
                    FieldName = mf.FieldName,
                    Formula = mf.Formula
                }))
                .GroupBy(mf => new { mf.CodDiv, mf.CodMeasure, mf.FieldName })
                .Select(g => g.First())
                .ToList();

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                // Save Master Data first (Measure Fields) - Skip if already exists
                foreach (var mf in measureFields)
                {
                    if (!await _repository.ExistsMeasureFieldAsync(mf.CodDiv, mf.CodMeasure, mf.FieldName))
                    {
                        await _repository.AddMeasureFieldAsync(mf);
                    }
                }


                await _repository.AddAsync(action);
                await _repository.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
            }

            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // Rethrow to let the API handle the error response
            }

            return Unit.Value;

        }
    }
}
