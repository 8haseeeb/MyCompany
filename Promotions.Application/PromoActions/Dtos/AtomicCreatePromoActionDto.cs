using System.Collections.Generic;
using Promotions.Application.Participants.Dtos;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.Products.Dtos;

namespace Promotions.Application.PromoActions.Dtos
{
    public class AtomicCreatePromoActionDto : CreatePromoActionDto
    {
        public List<CreateParticipantDto> Participants { get; set; } = new();
        public List<CreateDeliveryPointDto> DeliveryPoints { get; set; } = new();
        public List<AtomicCreateProductDto> Products { get; set; } = new();
    }
}
