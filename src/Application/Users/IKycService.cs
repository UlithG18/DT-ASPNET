namespace DT_ASPNET.Application.Users;

public interface IKycService
{
    Task<KycResult> ProcessAsync(KycSubmitRequest req);
}