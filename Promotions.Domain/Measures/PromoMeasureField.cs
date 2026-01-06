namespace Promotions.Domain.Measures
{
    public class PromoMeasureField
    {
        // Foreign Keys to PromoProduct (Parent)
        public int IdAction { get; set; }
        public string CodProduct { get; set; } = null!;
        public int LevProduct { get; set; }
        public string CodDisplay { get; set; } = null!;

        // Primary Key fields
        public string CodDiv { get; set; } = null!;
        public string CodMeasure { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        
        // Properties
        public string Formula { get; set; } = null!;

        // Navigation Property
        public virtual Products.PromoProduct Product { get; set; } = null!;
    }
}
