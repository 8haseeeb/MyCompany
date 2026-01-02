using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.Participants.Dtos
{
    public class ParticipantDto
    {
        public int IdAction { get; set; }
        public string CodParticipant { get; set; } = null!;
        public bool FlgInclusion { get; set; }
    }

}
