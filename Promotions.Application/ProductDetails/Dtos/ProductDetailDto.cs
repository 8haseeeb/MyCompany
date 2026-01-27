using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Promotions.Application.PromoArticles.Dtos;

namespace Promotions.Application.ProductDetails.Dtos
{
    public class ProductDetailDto
    {
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        public virtual ICollection<PromoArticleDto> Articles { get; set; } = new List<PromoArticleDto>();
    }
}
