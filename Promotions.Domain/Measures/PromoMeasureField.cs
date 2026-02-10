namespace Promotions.Domain.Measures
{
    public class PromoMeasureField
    {
        public string CodDiv { get; private set; } = null!;
        public string CodMeasure { get; private set; } = null!;
        public string FieldName { get; private set; } = null!;
        public string Formula { get; private set; } = null!;

        private PromoMeasureField() { }

        public PromoMeasureField(string codDiv, string codMeasure, string fieldName, string formula)
        {
            if (string.IsNullOrWhiteSpace(codDiv)) throw new ArgumentException("CodDiv is required.");
            if (string.IsNullOrWhiteSpace(codMeasure)) throw new ArgumentException("CodMeasure is required.");
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentException("FieldName is required.");

            CodDiv = codDiv;
            CodMeasure = codMeasure;
            FieldName = fieldName;
            Formula = formula;
        }

        public void UpdateFormula(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula)) throw new ArgumentException("Formula cannot be empty.");
            Formula = formula;
        }
    }
}


