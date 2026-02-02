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
        var promoActionsList = await _promoActions.GetAllAsync();
        var productsList = await _products.GetAllAsync();
        var allParticipants = await _participants.GetAllAsync();

        var now = DateTime.UtcNow;

        var activeCount = promoActionsList.Count(p => p.DteStartSellIn <= now && p.DteEndSellIn >= now);
        var pendingCount = promoActionsList.Count(p => p.DteStartSellIn > now);
        var expiredCount = promoActionsList.Count(p => p.DteEndSellIn < now);

        // Calculate Activity Trend (Last 30 Days)
        var trend = new List<DailyActivityDto>();
        for (int i = 0; i < 30; i += 3) // Every 3rd day to match UI graph density
        {
            var date = now.AddDays(-i);
            var active = promoActionsList.Count(p => p.DteStartSellIn <= date && p.DteEndSellIn >= date);
            var pending = promoActionsList.Count(p => p.DteStartSellIn > date);
            var failed = promoActionsList.Count(p => p.DteEndSellIn < date);

            trend.Add(new DailyActivityDto
            {
                DateLabel = date.ToString("dd"), // Just the day number e.g. "02"
                Active = active,
                Pending = pending,
                Failed = failed
            });
        }
        
        // Reverse so it goes from old -> new (Left to Right on Graph)
        trend.Reverse();

        return new DashboardMetricsDto
        {
            TotalPromotions = promoActionsList.Count,
            TotalProducts = productsList.Count,
            TotalParticipants = allParticipants.Count,
            ActiveActions = activeCount,
            PendingActions = pendingCount,
            ExpiredActions = expiredCount,
            EstimatedRevenue = 0,
            ActivityTrend = trend
        };
    }
}
