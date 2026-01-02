namespace Promotions.Application.CustomerRelations.Dtos
{
    public class CustomerRelationDto
    {
        public string CodHier { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public int IdLevel { get; set; }
        public DateTime DteStart { get; set; }

        public string CodParentNode { get; set; } = null!;
        public DateTime? DteEnd { get; set; }
    }
}
