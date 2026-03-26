namespace Promotions.Application.Common.Exceptions;

/// <summary>Thrown when IdAction / CodDiv (or related FK) references are invalid for create/update.</summary>
public sealed class InvalidPromotionReferenceException : Exception
{
    public InvalidPromotionReferenceException(string message) : base(message)
    {
    }

    public InvalidPromotionReferenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
