using System;

namespace Promotions.Application.Participants.Dtos
{
    public class CreateParticipantDto
    {
        public string CodParticipant { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        // Foreign Keys for CustomerRelation
        public string CodHier { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public int IdLevel { get; set; }
        public DateTime DteStart { get; set; }
    }
}
