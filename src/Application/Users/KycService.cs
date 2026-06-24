using DT_ASPNET.Domain.Users;

namespace DT_ASPNET.Application.Users;

public record KycSubmitRequest(Guid UserId, Stream DocumentImage, string FileName);
public record KycResult(bool Approved, string? RejectionReason);

// Interfaz para el adaptador de IA (implementada en Infrastructure/Kyc)
public interface IKycAiProvider
{
    Task<KycExtractedData?> ExtractAsync(Stream image, string fileName);
}

public record KycExtractedData(
    string FirstName, string LastName,
    string DocumentNumber, DateOnly DateOfBirth);

public class KycService(IUserRepository users, IKycAiProvider aiProvider)
{
    public async Task<KycResult> ProcessAsync(KycSubmitRequest req)
    {
        var user = await users.GetByIdAsync(req.UserId)
            ?? throw new InvalidOperationException("User not found.");

        if (user.KycStatus == KycStatus.Approved)
            return new KycResult(true, null);

        user.KycStatus = KycStatus.Pending;
        await users.SaveChangesAsync();

        // El archivo se procesa en memoria — nunca se persiste en disco
        var extracted = await aiProvider.ExtractAsync(req.DocumentImage, req.FileName);

        if (extracted is null)
        {
            user.KycStatus = KycStatus.Rejected;
            user.KycRejectionReason = "Could not extract data from document. Please try again.";
            await users.SaveChangesAsync();
            return new KycResult(false, user.KycRejectionReason);
        }

        // Guardar solo los datos extraídos, no el archivo
        user.KycStatus = KycStatus.Approved;
        user.KycDocumentNumber = extracted.DocumentNumber;
        user.KycDateOfBirth = extracted.DateOfBirth;
        user.KycRejectionReason = null;

        await users.SaveChangesAsync();
        return new KycResult(true, null);
    }
}
