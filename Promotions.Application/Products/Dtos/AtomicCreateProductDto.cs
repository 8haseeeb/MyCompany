using System.Collections.Generic;
using System.Text.Json.Serialization;
using Promotions.Application.ProductDetails.Dtos;

namespace Promotions.Application.Products.Dtos
{
    public class AtomicCreateProductDto : CreateProductDto
    {
        [JsonIgnore]
        public new int? IdAction { get; set; }

        public List<AtomicCreateProductDetailDto> Details { get; set; } = new();
        public List<CreatePromoMeasureFieldDto> MeasureFields { get; set; } = new();
    }
}


