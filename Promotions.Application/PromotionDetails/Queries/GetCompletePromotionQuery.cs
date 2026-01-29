using MediatR;

namespace Promotions.Application.PromotionDetails.Queries
{
    public class GetCompletePromotionQuery : IRequest<global::Promotions.Application.PromotionDetails.Dtos.CompletePromotionDto>
    {
        public int IdAction { get; set; }

        public GetCompletePromotionQuery(int idAction)
        {
            IdAction = idAction;
        }
    }
}
