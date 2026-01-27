using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.Measures.Dtos;

namespace Promotions.Application.Products.Dtos
{
    public class ProductDto
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

        public virtual ICollection<ProductDetailDto> Details { get; set; } = new List<ProductDetailDto>();
        public virtual ICollection<PromoMeasureFieldDto> MeasureFields { get; set; } = new List<PromoMeasureFieldDto>();
    }
}
