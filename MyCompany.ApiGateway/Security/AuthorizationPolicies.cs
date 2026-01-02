using Microsoft.AspNetCore.Authorization;

namespace MyCompany.ApiGateway.Security;

public static class AuthorizationPolicies
{
    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy("AuthenticatedUser", policy =>
        {
            policy.RequireAuthenticatedUser();
        });
    }
}
