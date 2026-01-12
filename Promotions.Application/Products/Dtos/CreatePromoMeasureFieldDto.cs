namespace Promotions.Application.Products.Dtos
{
    public class CreatePromoMeasureFieldDto
    {
        public string FieldName { get; set; } = null!;
        public string Formula { get; set; } = null!;
        
        // CodDiv and CodMeasure are usually inherited from the Product
    }
}
