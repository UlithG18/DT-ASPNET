namespace DT_ASPNET.Domain.User;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsOwner { get; set; } = false;

    public KycStatus KycStatus { get; set; } = KycStatus.NotSubmitted;
    public string? KycDocumentNumber { get; set; }
    public DateOnly? KycDateOfBirth { get; set; }
    public string? KycRejectionReason { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
