using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.Measures.Dtos
{
    public class PromoMeasureFieldDto
    {
        public string CodDiv { get; set; } = null!;
        public string CodMeasure { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        public string Formula { get; set; } = null!;
    }


}
