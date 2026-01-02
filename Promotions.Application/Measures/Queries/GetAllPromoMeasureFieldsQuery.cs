using MediatR;
using Promotions.Application.Measures.Dtos;
using System.Collections.Generic;

namespace Promotions.Application.Measures.Queries
{
    public class GetAllPromoMeasureFieldsQuery
        : IRequest<List<PromoMeasureFieldDto>>
    {
    }
}
