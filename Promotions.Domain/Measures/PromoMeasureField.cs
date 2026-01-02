namespace Promotions.Domain.Measures
{
    public class PromoMeasureField
    {
        public string CodDiv { get; }
        public string CodMeasure { get; }
        public string FieldName { get; }
        public string Formula { get; private set; } // private setter

        public PromoMeasureField(string codDiv, string codMeasure, string fieldName, string formula)
        {
            CodDiv = codDiv ?? throw new ArgumentNullException(nameof(codDiv));
            CodMeasure = codMeasure ?? throw new ArgumentNullException(nameof(codMeasure));
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            Formula = formula ?? throw new ArgumentNullException(nameof(formula));
        }

        public void UpdateFormula(string newFormula)
        {
            Formula = newFormula ?? throw new ArgumentNullException(nameof(newFormula));
        }
    }
}
