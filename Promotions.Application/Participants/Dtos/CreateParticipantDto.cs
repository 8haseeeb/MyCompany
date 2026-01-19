using System;

namespace Promotions.Application.Participants.Dtos
{
    public class CreateParticipantDto
    {
        public string CodParticipant { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        // Foreign Keys for CustomerRelation (Made optional for simplified UI)
        public string? CodHier { get; set; }
        public string? CodDiv { get; set; }
        public string? CodNode { get; set; }
        public int? IdLevel { get; set; }
        public DateTime? DteStart { get; set; }
    }
}
