namespace Promotions.Application.DeliveryPoints.Dtos
{
    public class CreateDeliveryPointDto
    {
        public string CodDeliveryPoint { get; set; } = null!;
        public bool FlgInclusion { get; set; }

        
        // Foreign Keys for CustomerRelation
        public string CodHier { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string CodNode { get; set; } = null!;
        public int IdLevel { get; set; }
        public DateTime DteStart { get; set; }
    }
}
