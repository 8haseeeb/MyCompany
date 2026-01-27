using Promotions.Application.ProductDetails.Dtos;

namespace Promotions.Application.Products.Dtos
{
    public class CreateProductDto
    {
        public int? IdAction { get; set; }
        public string? CodProduct { get; set; }
        public int? LevProduct { get; set; }
        public string? CodDisplay { get; set; }

        public string CodDiv { get; set; } = null!;
        public decimal QtyEstimated { get; set; }
        public decimal? PerceDiscount1 { get; set; }
        public decimal? PerceDiscount2 { get; set; }
        public decimal? NumMeasure { get; set; }
        public string? CodMeasure { get; set; }
        public List<AtomicCreateProductDetailDto> Details { get; set; } = new();
        public List<CreatePromoMeasureFieldDto> MeasureFields { get; set; } = new();
    }
}
