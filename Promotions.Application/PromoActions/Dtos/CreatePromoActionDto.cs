using System;

namespace Promotions.Application.PromoActions.Dtos
{
    public class CreatePromoActionDto
    {
        public int IdAction { get; set; }         
        public string Name { get; set; } = null!; // DESACTION

        public string CodDiv { get; set; } = null!;
        public string CodContractor { get; set; } = null!;
        
        public string? ContractorCodHier { get; set; }
        public int? ContractorIdLevel { get; set; }
        public DateTime? ContractorDteStart { get; set; }

        public DateTime DteStartSellIn { get; set; }
        public DateTime DteEndSellIn { get; set; }

        public DateTime DteStartSellOut { get; set; }
        public DateTime DteEndSellOut { get; set; }

        public string? DocumentKey { get; set; }
        public DateTime? DteToShost { get; set; }

        public int? LevParticipants { get; set; }
    }
}
