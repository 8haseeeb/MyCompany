using AutoMapper;
using MediatR;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Measures;
using Promotions.Application.Common.Interfaces;
using Promotions.Application.PromoActions.Commands;
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

        public CreateAtomicPromoActionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateAtomicPromoActionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var allRelations = await _unitOfWork.CustomerRelations.GetAllAsync();
            var allParticipants = await _unitOfWork.Participants.GetAllAsync();
            var allDps = await _unitOfWork.DeliveryPoints.GetAllAsync();

            // Resolve missing hierarchy data for Participants
            foreach (var p in dto.Participants)
            {
                if (string.IsNullOrEmpty(p.CodHier) || string.IsNullOrEmpty(p.CodNode))
                {
                    var match = allRelations.FirstOrDefault(r => 
                        r.CodNode.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase) || 
                        r.CodHier.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase));
                    
                    if (match == null)
                    {
                        var pMatch = allParticipants.FirstOrDefault(ap => ap.CodParticipant.Equals(p.CodParticipant, StringComparison.OrdinalIgnoreCase));
                        if (pMatch != null)
                        {
                            p.CodHier = pMatch.CodHier;
                            p.CodDiv = pMatch.CodDiv;
                            p.CodNode = pMatch.CodNode;
                            p.IdLevel = pMatch.IdLevel;
                            p.DteStart = pMatch.DteStart;
                            continue;
                        }

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
                        r.CodNode.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase) || 
                        r.CodHier.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase));
                    
                    if (match == null)
                    {
                        var dpMatch = allDps.FirstOrDefault(adp => adp.CodDeliveryPoint.Equals(dp.CodDeliveryPoint, StringComparison.OrdinalIgnoreCase));
                        if (dpMatch != null)
                        {
                            dp.CodHier = dpMatch.CodHier;
                            dp.CodDiv = dpMatch.CodDiv;
                            dp.CodNode = dpMatch.CodNode;
                            dp.IdLevel = dpMatch.IdLevel;
                            dp.DteStart = dpMatch.DteStart;
                            continue;
                        }

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
                        throw new KeyNotFoundException($"Delivery Point hierarchy could not be resolved for code: {dp.CodDeliveryPoint}.");
                    }
                }
            }

            // Generate an IdAction if 0 (Needed for Rich Domain Model construction)
            if (dto.IdAction == 0)
            {
                var maxId = await _unitOfWork.PromoActions.GetMaxIdAsync();
                dto.IdAction = maxId + 1;
            }

            // Map DTO to Entity using AutoMapper with context to pass IdAction down to nested children
            var action = _mapper.Map<PromoAction>(dto, opt => opt.Items["IdAction"] = dto.IdAction);

            // Collect and Map Master Measure Fields
            var measureFields = dto.Products
                .Where(p => p.MeasureFields != null && p.MeasureFields.Any())
                .SelectMany(p => p.MeasureFields.Select(mf => new PromoMeasureField(
                    p.CodDiv!,
                    p.CodMeasure ?? string.Empty,
                    mf.FieldName!,
                    mf.Formula!
                )))
                .GroupBy(mf => new { mf.CodDiv, mf.CodMeasure, mf.FieldName })
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
