using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.PromoArticles.Dtos
{
    public class CreatePromoArticleDto
    {
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;

        public string? CodNode1 { get; set; }
        public string? CodNode2 { get; set; }
        public string? CodNodeN { get; set; }
    }
}
