using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Domain.Articles
{

    public class PromoArticle
    {
        public int IdAction { get; private set; }
        public string CodProduct { get; private set; } = null!;
        public int LevProduct { get; private set; }
        public string CodDisplay { get; private set; } = null!;
        public string CodDiv { get; private set; } = null!;
        public string CodNode { get; private set; } = null!;
        public string? CodNode1 { get; private set; }
        public string? CodNode2 { get; private set; }
        public string? CodNodeN { get; private set; }

        public virtual ProductDetails.PromoProductDetail ProductDetail { get; private set; } = null!;

        private PromoArticle() { }

        public PromoArticle(int idAction, string codProduct, int levProduct, string codDisplay, string codDiv, string codNode, string? codNode1, string? codNode2, string? codNodeN)
        {
            IdAction = idAction;
            CodProduct = codProduct;
            LevProduct = levProduct;
            CodDisplay = codDisplay;
            CodDiv = codDiv;
            CodNode = codNode;
            CodNode1 = codNode1;
            CodNode2 = codNode2;
            CodNodeN = codNodeN;
        }

        public void UpdateNodes(string? node1, string? node2, string? nodeN)
        {
            CodNode1 = node1;
            CodNode2 = node2;
            CodNodeN = nodeN;
        }
    }
}
