using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Measures;
using Promotions.Domain.Articles;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.Common.Exceptions;
using Promotions.Application.PromoActions.Commands;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.Products.Dtos;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class CreateAtomicPromoActionCommandHandler : IRequestHandler<CreateAtomicPromoActionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateAtomicPromoActionCommandHandler> _logger;

        public CreateAtomicPromoActionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateAtomicPromoActionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateAtomicPromoActionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            _logger.LogInformation("[CreateAtomic] Starting atomic promo action creation. IdAction: {IdAction}, Products: {ProductCount}",
                dto.IdAction, dto.Products?.Count ?? 0);

            await ValidateIdActionAndDivisionsAsync(dto, cancellationToken);

            var allRelations = await _unitOfWork.CustomerRelations.GetAllAsync();
            var allParticipants = await _unitOfWork.Participants.GetAllAsync();
            var allDps = await _unitOfWork.DeliveryPoints.GetAllAsync();

            // Resolve missing hierarchy data for Participants
            foreach (var p in dto.Participants)
            {
                if (string.IsNullOrEmpty(p.CodHier) || string.IsNullOrEmpty(p.CodNode))
                {
                    var match = allRelations.FirstOrDefault(r => 
                        (r.CodNode != null && r.CodNode.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase)) || 
                        (r.CodHier != null && r.CodHier.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase)));
                    
                    if (match == null)
                    {
                        var pMatch = allParticipants.FirstOrDefault(ap => ap.CodParticipant != null && ap.CodParticipant.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase));
                        if (pMatch != null)
                        {
                            p.CodHier = pMatch.CodHier;
                            p.CodDiv = pMatch.CodDiv;
                            p.CodNode = pMatch.CodNode;
                            p.IdLevel = pMatch.IdLevel;
                            p.DteStart = pMatch.DteStart;
                            continue;
                        }

                        var suffix = p.CodParticipant != null && p.CodParticipant.Contains("-") ? p.CodParticipant.Split('-').Last() : p.CodParticipant;
                        match = allRelations.FirstOrDefault(r => 
                            (r.CodNode != null && (r.CodNode.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) || r.CodNode.Equals(suffix, StringComparison.OrdinalIgnoreCase))) ||
                            (r.CodHier != null && (r.CodHier.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) || r.CodHier.Equals(suffix, StringComparison.OrdinalIgnoreCase))));
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
                        _logger.LogError("[CreateAtomic] Participant hierarchy could not be resolved. CodParticipant: {CodParticipant}", p.CodParticipant);
                        throw new KeyNotFoundException($"Participant hierarchy could not be resolved for code: {p.CodParticipant}.");
                    }
                }
            }

            // Resolve missing hierarchy data for Delivery Points
            foreach (var dp in dto.DeliveryPoints)
            {
                if (string.IsNullOrEmpty(dp.CodHier) || string.IsNullOrEmpty(dp.CodNode))
                {
                    var match = allRelations.FirstOrDefault(r => 
                        (r.CodNode != null && r.CodNode.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase)) || 
                        (r.CodHier != null && r.CodHier.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase)));
                    
                    if (match == null)
                    {
                        var dpMatch = allDps.FirstOrDefault(adp => adp.CodDeliveryPoint != null && adp.CodDeliveryPoint.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase));
                        if (dpMatch != null)
                        {
                            dp.CodHier = dpMatch.CodHier;
                            dp.CodDiv = dpMatch.CodDiv;
                            dp.CodNode = dpMatch.CodNode;
                            dp.IdLevel = dpMatch.IdLevel;
                            dp.DteStart = dpMatch.DteStart;
                            continue;
                        }

                        var suffix = dp.CodDeliveryPoint != null && dp.CodDeliveryPoint.Contains("-") ? dp.CodDeliveryPoint.Split('-').Last() : dp.CodDeliveryPoint;
                        match = allRelations.FirstOrDefault(r => 
                            (r.CodNode != null && (r.CodNode.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) || r.CodNode.Equals(suffix, StringComparison.OrdinalIgnoreCase))) ||
                            (r.CodHier != null && (r.CodHier.EndsWith("-" + suffix, StringComparison.OrdinalIgnoreCase) || r.CodHier.Equals(suffix, StringComparison.OrdinalIgnoreCase))));
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
                        _logger.LogError("[CreateAtomic] Delivery Point hierarchy could not be resolved. CodDeliveryPoint: {CodDeliveryPoint}", dp.CodDeliveryPoint);
                        throw new KeyNotFoundException($"Delivery Point hierarchy could not be resolved for code: {dp.CodDeliveryPoint}.");
                    }
                }
            }

            // Generate an IdAction if 0 (Needed for Rich Domain Model construction)
            if (dto.IdAction == 0)
            {
                var maxId = await _unitOfWork.PromoActions.GetMaxIdAsync();
                dto.IdAction = maxId + 1;
                _logger.LogInformation("[CreateAtomic] Auto-generated IdAction: {IdAction}", dto.IdAction);
            }

            // Map DTO to Entity using AutoMapper with context to pass IdAction down to nested children
            var action = _mapper.Map<PromoAction>(dto, opt => opt.Items["IdAction"] = dto.IdAction);

            // Collect and Map Master Measure Fields (Fix 500 error: filter null measures)
            var measureFields = dto.Products
                .Where(p => p.MeasureFields != null && p.MeasureFields.Any() && !string.IsNullOrWhiteSpace(p.CodMeasure))
                .SelectMany(p => p.MeasureFields.Select(mf => new PromoMeasureField(
                    p.CodDiv!,
                    p.CodMeasure!.Trim(),
                    mf.FieldName!,
                    mf.Formula!
                )))
                .GroupBy(mf => new { mf.CodDiv, CodMeasure = mf.CodMeasure.Trim(), mf.FieldName })
                .Select(g => g.First())
                .ToList();

            foreach (var mf in measureFields)
            {
                if (!await _unitOfWork.PromoActions.ExistsMeasureFieldAsync(mf.CodDiv, mf.CodMeasure, mf.FieldName))
                {
                    await _unitOfWork.PromoActions.AddMeasureFieldAsync(mf);
                }
            }

            await _unitOfWork.PromoActions.AddAsync(action);

            // Manually save standalone articles associated with the promotion
            var articlesToSave = new List<PromoArticle>();
            foreach (var prodDto in dto.Products)
            {
                foreach (var detailDto in prodDto.Details)
                {
                    if (detailDto.Articles != null)
                    {
                        foreach (var artDto in detailDto.Articles)
                        {
                            articlesToSave.Add(new PromoArticle(
                                action.IdAction,
                                prodDto.CodProduct ?? string.Empty,
                                prodDto.LevProduct ?? 0,
                                prodDto.CodDisplay ?? string.Empty,
                                detailDto.CodDiv ?? action.CodDiv ?? string.Empty,
                                detailDto.CodNode ?? string.Empty,
                                artDto.CodNode1,
                                artDto.CodNode2,
                                artDto.CodNodeN
                            ));
                        }
                    }
                }
            }

            foreach (var art in articlesToSave)
            {
                if (await _unitOfWork.PromoArticles.GetByIdAsync(art.CodDiv, art.CodNode) == null)
                {
                    await _unitOfWork.PromoArticles.AddAsync(art);
                }
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (LooksLikeForeignKeyConstraint(ex))
            {
                _logger.LogWarning(ex,
                    "[CreateAtomic] FK violation on SaveChanges (SQL 547). IdAction: {IdAction}, CodDiv: {CodDiv}",
                    dto.IdAction, dto.CodDiv);
                throw new InvalidPromotionReferenceException("Invalid IdAction or CodDiv", ex);
            }

            _logger.LogInformation("[CreateAtomic] Promo action created successfully. IdAction: {IdAction}", dto.IdAction);
            return Unit.Value;
        }

        /// <summary>
        /// IdAction: for creates, non-zero must not already exist (PK). CodDiv: must appear on TA501DELIVERYPOINTS or customer relations (TB0042).
        /// </summary>
        private async Task ValidateIdActionAndDivisionsAsync(AtomicCreatePromoActionDto dto, CancellationToken cancellationToken)
        {
            if (dto.IdAction != 0 && await _unitOfWork.PromoActions.ExistsIdActionAsync(dto.IdAction))
            {
                _logger.LogWarning("[CreateAtomic] IdAction already exists in TA500PROMOACTION: {IdAction}", dto.IdAction);
                throw new InvalidPromotionReferenceException("Invalid IdAction or CodDiv");
            }

            var codDivs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(dto.CodDiv))
                codDivs.Add(dto.CodDiv.Trim());
            foreach (var p in dto.Products ?? new List<AtomicCreateProductDto>())
            {
                if (!string.IsNullOrWhiteSpace(p.CodDiv))
                    codDivs.Add(p.CodDiv.Trim());
            }

            foreach (var codDiv in codDivs)
            {
                var inDeliveryPoints = await _unitOfWork.DeliveryPoints.AnyWithCodDivAsync(codDiv);
                var inRelations = await _unitOfWork.CustomerRelations.AnyWithCodDivAsync(codDiv);
                if (!inDeliveryPoints && !inRelations)
                {
                    _logger.LogWarning(
                        "[CreateAtomic] CodDiv not referenced in TA501DELIVERYPOINTS or customer relations: {CodDiv}",
                        codDiv);
                    throw new InvalidPromotionReferenceException("Invalid IdAction or CodDiv");
                }
            }
        }

        /// <summary>Detect FK failures without referencing SqlClient from Application layer.</summary>
        private static bool LooksLikeForeignKeyConstraint(DbUpdateException ex)
        {
            for (var e = (Exception?)ex; e != null; e = e.InnerException)
            {
                if (e.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
