using MediatR;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Participants;
using Promotions.Domain.Products;
using Promotions.Domain.DeliveryPoints;
using Promotions.Domain.ProductDetails;
using Promotions.Domain.Articles;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Commands.Handlers
{
    public class CreateAtomicPromoActionCommandHandler : IRequestHandler<CreateAtomicPromoActionCommand, Unit>
    {
        private readonly IPromoActionRepository _repository;

        public CreateAtomicPromoActionCommandHandler(IPromoActionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateAtomicPromoActionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

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
                    CodHier = p.CodHier,
                    CodDiv = p.CodDiv,
                    CodNode = p.CodNode,
                    IdLevel = p.IdLevel,
                    DteStart = p.DteStart
                }).ToList(),

                // Map Delivery Points
                DeliveryPoints = dto.DeliveryPoints.Select(dp => new PromoDeliveryPoint
                {
                    IdAction = dto.IdAction,
                    CodDeliveryPoint = dp.CodDeliveryPoint,
                    FlgInclusion = dp.FlgInclusion,
                    CodHier = dp.CodHier,
                    CodDiv = dp.CodDiv,
                    CodNode = dp.CodNode,
                    IdLevel = dp.IdLevel,
                    DteStart = dp.DteStart
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

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
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
