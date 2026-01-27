using System;

namespace Promotions.Application.Products.Dtos
{
    public class UpdateProductDto
    {
        public string? CodDiv { get; set; }
        public decimal? QtyEstimated { get; set; }
        public decimal? PerceDiscount1 { get; set; }
        public decimal? PerceDiscount2 { get; set; }
        public decimal? NumMeasure { get; set; }
        public string? CodMeasure { get; set; }
    }
}
