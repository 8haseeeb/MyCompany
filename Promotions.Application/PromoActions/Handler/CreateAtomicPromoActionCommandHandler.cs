using AutoMapper;
using MediatR;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Measures;
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class CreateAtomicPromoActionCommandHandler : IRequestHandler<CreateAtomicPromoActionCommand, Unit>
    {
        private readonly IPromoActionRepository _repository;
        private readonly CustomerRelations.Interfaces.ICustomerRelationRepository _relationRepository;
        private readonly Participant.Interfaces.IParticipantRepository _participantRepository;
        private readonly DeliveryPoints.Interfaces.IDeliveryPointRepository _dpRepository;
        private readonly IMapper _mapper;

        public CreateAtomicPromoActionCommandHandler(
            IPromoActionRepository repository,
            CustomerRelations.Interfaces.ICustomerRelationRepository relationRepository,
            Participant.Interfaces.IParticipantRepository participantRepository,
            DeliveryPoints.Interfaces.IDeliveryPointRepository dpRepository,
            IMapper mapper)
        {
            _repository = repository;
            _relationRepository = relationRepository;
            _participantRepository = participantRepository;
            _dpRepository = dpRepository;
            _mapper = mapper;
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

            // Map DTO to Entity using AutoMapper
            var action = _mapper.Map<PromoAction>(dto);

            // Collect and Map Master Measure Fields
            var measureFields = dto.Products
                .Where(p => p.MeasureFields != null && p.MeasureFields.Any())
                .SelectMany(p => p.MeasureFields.Select(mf => new PromoMeasureField
                {
                    CodDiv = p.CodDiv!,
                    CodMeasure = p.CodMeasure ?? string.Empty,
                    FieldName = mf.FieldName!,
                    Formula = mf.Formula!
                }))
                .GroupBy(mf => new { mf.CodDiv, mf.CodMeasure, mf.FieldName })
                .Select(g => g.First())
                .ToList();

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
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
                throw;
            }

            return Unit.Value;
        }
    }
}
