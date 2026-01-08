using System.Text.Json.Serialization;

namespace Promotions.Application.PromoArticles.Dtos
{
    public class AtomicCreatePromoArticleDto : CreatePromoArticleDto
    {
        [JsonIgnore]
        public new int? IdAction { get; set; }
        [JsonIgnore]
        public new string? CodProduct { get; set; }
        [JsonIgnore]
        public new int? LevProduct { get; set; }
        [JsonIgnore]
        public new string? CodDisplay { get; set; }
    }
}
