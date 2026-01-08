using System.Collections.Generic;
using System.Text.Json.Serialization;
using Promotions.Application.PromoArticles.Dtos;

namespace Promotions.Application.ProductDetails.Dtos
{
    public class AtomicCreateProductDetailDto : CreateProductDetailDto
    {
        [JsonIgnore]
        public new int? IdAction { get; set; }
        [JsonIgnore]
        public new string? CodProduct { get; set; }
        [JsonIgnore]
        public new int? LevProduct { get; set; }
        [JsonIgnore]
        public new string? CodDisplay { get; set; }

        public List<AtomicCreatePromoArticleDto> Articles { get; set; } = new();
    }
}

