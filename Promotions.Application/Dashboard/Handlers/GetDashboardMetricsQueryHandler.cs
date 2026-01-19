using MediatR;
using Promotions.Application.Dashboard.Dtos;
using Promotions.Application.Dashboard.Queries;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.Participant.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Promotions.Application.Dashboard.Handlers;

public class GetDashboardMetricsQueryHandler : IRequestHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    private readonly IPromoActionRepository _promoActions;
    private readonly IProductRepository _products;
    private readonly IParticipantRepository _participants;

    public GetDashboardMetricsQueryHandler(
        IPromoActionRepository promoActions,
        IProductRepository products,
        IParticipantRepository participants)
    {
        _promoActions = promoActions;
        _products = products;
        _participants = participants;
    }

    public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        // Fetch data using repositories to avoid Infrastructure dependency
        var promoActionsList = await _promoActions.GetAllAsync();
        var productsList = await _products.GetAllAsync();
        var allParticipants = await _participants.GetAllAsync();

        return new DashboardMetricsDto
        {
            TotalPromotions = promoActionsList.Count,
            TotalProducts = productsList.Count,
            TotalParticipants = allParticipants.Count,
            ActiveActions = promoActionsList.Count, // Logic placeholder
            EstimatedRevenue = 0 // Logic placeholder
        };
    }
}
