using MediatR;

namespace Promotions.Application.Measures.Commands
{
    public class UpdatePromoMeasureFieldCommand : IRequest<Unit>
    {
        public string CodMeasure { get; }
        public string CodDiv { get; }
        public string FieldName { get; }
        public string Formula { get; }

        public UpdatePromoMeasureFieldCommand(
            string codMeasure,
            string codDiv,
            string fieldName,
            string formula)
        {
            CodMeasure = codMeasure;
            CodDiv = codDiv;
            FieldName = fieldName;
            Formula = formula;
        }
    }
}
