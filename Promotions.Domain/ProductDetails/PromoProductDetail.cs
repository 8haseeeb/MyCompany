using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Domain.ProductDetails
{
    public class PromoProductDetail
    {
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public string CodDiv { get; set; } = null!;

        public bool FlgInclusion { get; set; }

        // Navigation Properties
        public virtual Products.PromoProduct Product { get; set; } = null!;
        public virtual ICollection<Articles.PromoArticle> Articles { get; set; } = new List<Articles.PromoArticle>();
    }
}
