using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.Measures.Dtos
{
    public class PromoMeasureFieldDto
    {
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodMeasure { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        public string Formula { get; set; } = null!;
    }

}
