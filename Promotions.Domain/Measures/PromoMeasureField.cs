namespace Promotions.Domain.Measures
{
    public class PromoMeasureField
    {
        // Primary Key fields
        public string CodDiv { get; set; } = null!;
        public string CodMeasure { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        
        // Properties
        public string Formula { get; set; } = null!;
    }
}


