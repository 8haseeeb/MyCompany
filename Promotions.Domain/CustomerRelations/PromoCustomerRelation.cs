using System;

namespace Promotions.Domain.CustomerRelations
{
    public class CustomerRelation
    {
        public string CodHier { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public int IdLevel { get; set; }
        public DateTime DteStart { get; set; }

        // UPDATABLE
        public string CodParentNode { get; set; } = null!;
        public DateTime? DteEnd { get; set; }
    }
}
