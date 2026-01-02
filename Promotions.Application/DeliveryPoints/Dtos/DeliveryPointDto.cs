using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.DeliveryPoints.Dtos
{
    public class DeliveryPointDto
    {
        public int IdAction { get; set; }
        public string CodDeliveryPoint { get; set; } = null!;
        public bool FlgInclusion { get; set; }
    }
}
