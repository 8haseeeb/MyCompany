using MediatR;
using Promotions.Application.Interfaces;
using Promotions.Application.Measures.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promotions.Application.Measures.Queries
{
    public class GetAllPromoMeasureFieldsQueryHandler
        : IRequestHandler<GetAllPromoMeasureFieldsQuery, List<PromoMeasureFieldDto>>
    {
        private readonly IPromoMeasureFieldRepository _repository;

        public GetAllPromoMeasureFieldsQueryHandler(
            IPromoMeasureFieldRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PromoMeasureFieldDto>> Handle(
            GetAllPromoMeasureFieldsQuery request,
            CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities.Select(x => new PromoMeasureFieldDto
            {
                CodDiv = x.CodDiv,
                CodMeasure = x.CodMeasure,
                FieldName = x.FieldName,
                Formula = x.Formula
            }).ToList();
        }
    }
}
