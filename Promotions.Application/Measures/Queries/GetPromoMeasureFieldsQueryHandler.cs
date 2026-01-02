using MediatR;
using Promotions.Application.Interfaces;
using Promotions.Application.Measures.Dtos;

namespace Promotions.Application.Measures.Queries
{
    public class GetPromoMeasureFieldQueryHandler
        : IRequestHandler<GetPromoMeasureFieldQuery, PromoMeasureFieldDto?>
    {
        private readonly IPromoMeasureFieldRepository _repository;

        public GetPromoMeasureFieldQueryHandler(IPromoMeasureFieldRepository repository)
        {
            _repository = repository;
        }

        public async Task<PromoMeasureFieldDto?> Handle(
            GetPromoMeasureFieldQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(
                request.CodDiv,
                request.CodMeasure,
                request.FieldName,
                cancellationToken);

            if (entity == null)
                return null;

            return new PromoMeasureFieldDto
            {
                CodDiv = entity.CodDiv,
                CodMeasure = entity.CodMeasure,
                FieldName = entity.FieldName,
                Formula = entity.Formula
            };
        }
    }
}
