using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Domain.ProductDetails
{
    public class PromoProductDetail
    {
        public int IdAction { get; private set; }
        public string CodProduct { get; private set; } = null!;
        public int LevProduct { get; private set; }
        public string CodDisplay { get; private set; } = null!;
        public string CodNode { get; private set; } = null!;
        public string CodDiv { get; private set; } = null!;
        public bool FlgInclusion { get; private set; }

        // Navigation Properties
        public virtual Products.PromoProduct Product { get; private set; } = null!;
        public virtual Articles.PromoArticle Article { get; private set; } = null!;

        private PromoProductDetail() { }

        public PromoProductDetail(int idAction, string codProduct, int levProduct, string codDisplay, string codNode, string codDiv, bool flgInclusion)
        {
            IdAction = idAction;
            CodProduct = codProduct;
            LevProduct = levProduct;
            CodDisplay = codDisplay;
            CodNode = codNode;
            CodDiv = codDiv;
            FlgInclusion = flgInclusion;
        }


        public void UpdateInclusion(bool flgInclusion)
        {
            FlgInclusion = flgInclusion;
        }
    }
}
