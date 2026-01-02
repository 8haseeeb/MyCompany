using MediatR;

namespace Promotions.Application.Measures.Commands
{
    public class DeletePromoMeasureFieldCommand : IRequest<Unit>
    {
        public string CodMeasure { get; set; } = null!;
        public string CodDiv { get; set; } = null!;
        public string FieldName { get; set; } = null!;

        public DeletePromoMeasureFieldCommand(string codMeasure, string codDiv, string fieldName)
        {
            CodMeasure = codMeasure;
            CodDiv = codDiv;
            FieldName = fieldName;
        }
    }
}
