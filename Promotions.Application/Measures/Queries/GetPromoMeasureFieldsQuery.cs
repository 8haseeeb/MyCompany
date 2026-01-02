using MediatR;
using Promotions.Application.Measures.Dtos;

namespace Promotions.Application.Measures.Queries
{
    public class GetPromoMeasureFieldQuery
        : IRequest<PromoMeasureFieldDto?>
    {
        public string CodMeasure { get; }
        public string CodDiv { get; }
        public string FieldName { get; }

        public GetPromoMeasureFieldQuery(
            string codMeasure,
            string codDiv,
            string fieldName)
        {
            CodMeasure = codMeasure;
            CodDiv = codDiv;
            FieldName = fieldName;
        }
    }
}
