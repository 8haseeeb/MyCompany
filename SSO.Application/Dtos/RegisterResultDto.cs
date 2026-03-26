namespace SSO.Application.Dtos;

/// <summary>Returned from public registration — no tokens; user must call login.</summary>
public class RegisterResultDto
{
    public string Message { get; set; } = "Account created. Please log in.";
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
