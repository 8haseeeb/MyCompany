namespace Promotions.Application.CustomerRelations.Dtos
{
    public class UpdateCustomerRelationDto
    {
        public string CodParentNode { get; set; } = null!;
        public DateTime? DteEnd { get; set; }
    }
}
