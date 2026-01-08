using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.PromoActions.Dtos
{
    public class PromoActionDto
    {
        public int IdAction { get; set; }
        public string Name { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public DateTime DteStartSellIn { get; set; }
        public DateTime DteEndSellIn { get; set; }
        public DateTime DteStartSellOut { get; set; }
        public DateTime DteEndSellOut { get; set; }
        public string? DocumentKey { get; set; }
        public DateTime? DteToShost { get; set; }
        public int? LevParticipants { get; set; }
    }
}
