namespace Promotions.Application.DeliveryPoints.Dtos
{
    public class CreateDeliveryPointDto
    {
        public string CodDeliveryPoint { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        
        // Foreign Keys for CustomerRelation (Made optional for simplified UI)
        public string? CodHier { get; set; }
        public string? CodDiv { get; set; }
        public string? CodNode { get; set; }
        public int? IdLevel { get; set; }
        public DateTime? DteStart { get; set; }
    }
}
