using System;

namespace Promotions.Application.PromoActions.Dtos
{
    public class CreatePromoActionDto
    {
        public int IdAction { get; set; }          // ID_ACTION (PK)
        public string Name { get; set; } = null!; // DESACTION

        public string CodDiv { get; set; } = null!;
        public string CodContractor { get; set; } = null!;

        public DateTime DteStartSellIn { get; set; }
        public DateTime DteEndSellIn { get; set; }

        public DateTime DteStartSellOut { get; set; }
        public DateTime DteEndSellOut { get; set; }

        public string? DocumentKey { get; set; }
        public DateTime? DteToShost { get; set; }

        public int? LevParticipants { get; set; }
    }
}
