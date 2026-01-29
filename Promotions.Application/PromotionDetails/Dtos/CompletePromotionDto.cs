using System.Collections.Generic;

namespace Promotions.Application.PromotionDetails.Dtos
{
    public class CompletePromotionDto
    {
        public global::Promotions.Application.PromoActions.Dtos.PromoActionDto? PromoAction { get; set; }
        public List<global::Promotions.Application.Products.Dtos.ProductDto> Products { get; set; } = new();
        public List<global::Promotions.Application.PromoArticles.Dtos.PromoArticleDto> Articles { get; set; } = new();
        public List<global::Promotions.Application.Measures.Dtos.PromoMeasureFieldDto> MeasureFields { get; set; } = new();
        public List<global::Promotions.Application.Participants.Dtos.ParticipantDto> Participants { get; set; } = new();
        public List<global::Promotions.Application.DeliveryPoints.Dtos.DeliveryPointDto> DeliveryPoints { get; set; } = new();
        public List<global::Promotions.Application.ProductDetails.Dtos.ProductDetailDto> ProductDetails { get; set; } = new();
        public List<global::Promotions.Application.CustomerRelations.Dtos.CustomerRelationDto> Customers { get; set; } = new();
    }
}
