using Microsoft.EntityFrameworkCore;

namespace SSO.Application.Common;

/// <summary>
/// Detects duplicate-key failures without referencing SqlClient from this layer.
/// </summary>
public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException ex)
    {
        for (var e = ex.InnerException; e != null; e = e.InnerException)
        {
            var msg = e.Message;
            if (msg.Contains("UNIQUE KEY constraint", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("duplicate key", StringComparison.OrdinalIgnoreCase))
                return true;
            if (msg.Contains("IX_", StringComparison.Ordinal) && msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
