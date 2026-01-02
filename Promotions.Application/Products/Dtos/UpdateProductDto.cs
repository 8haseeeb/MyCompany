using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.Products.Dtos
{
    public class UpdateProductDto
    {
        public string CodDiv { get; set; } = null!;
        public decimal QtyEstimated { get; set; }
        public decimal? PerceDiscount1 { get; set; }
        public decimal? PerceDiscount2 { get; set; }
        public decimal? NumMeasure { get; set; }
        public string? CodMeasure { get; set; }
    }
}
