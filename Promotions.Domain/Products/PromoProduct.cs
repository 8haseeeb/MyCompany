namespace Promotions.Domain.Products
{
    public class PromoProduct
    {
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;

        public string CodDiv { get; set; } = null!;
        public decimal QtyEstimated { get; set; }
        public decimal? PerceDiscount1 { get; set; }
        public decimal? PerceDiscount2 { get; set; }
        public decimal? NumMeasure { get; set; }
        public string? CodMeasure { get; set; }
    }
}
