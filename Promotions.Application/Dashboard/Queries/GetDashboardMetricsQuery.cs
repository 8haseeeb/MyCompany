using MediatR;
using Promotions.Application.Dashboard.Dtos;

namespace Promotions.Application.Dashboard.Queries;

public record GetDashboardMetricsQuery() : IRequest<DashboardMetricsDto>;
