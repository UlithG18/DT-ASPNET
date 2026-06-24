using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DT_ASPNET.Application.Users;
using Microsoft.Extensions.Configuration;

namespace DT_ASPNET.Infrastructure.Kyc;

public class AiKycProvider(IConfiguration config) : IKycAiProvider
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";

    public async Task<KycExtractedData?> ExtractAsync(Stream image, string fileName)
    {
        var apiKey = config["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return null;

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var mediaType = ext switch
        {
            ".png"  => "image/png",
            ".webp" => "image/webp",
            ".gif"  => "image/gif",
            _       => "image/jpeg"
        };

        var payload = new
        {
            model = "gpt-4o",
            max_tokens = 512,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "image_url",
                            image_url = new { url = $"data:{mediaType};base64,{base64}" }
                        },
                        new
                        {
                            type = "text",
                            text = """
                                Extrae los datos de este documento de identidad.
                                Responde ÚNICAMENTE con JSON puro, sin markdown ni texto adicional:
                                {"firstName":"...","lastName":"...","documentNumber":"...","dateOfBirth":"YYYY-MM-DD"}
                                Si no puedes leer algún campo con certeza, usa null en ese campo.
                                """
                        }
                    }
                }
            }
        };

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var json    = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await http.PostAsync(ApiUrl, content);
        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        var text = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            var extracted = JsonSerializer.Deserialize<KycRaw>(text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (extracted?.DocumentNumber is null)
                return null;

            return new KycExtractedData(
                extracted.FirstName    ?? "",
                extracted.LastName     ?? "",
                extracted.DocumentNumber,
                DateOnly.Parse(extracted.DateOfBirth ?? "1900-01-01"));
        }
        catch
        {
            return null;
        }
    }

    private record KycRaw(
        string? FirstName,
        string? LastName,
        string? DocumentNumber,
        string? DateOfBirth);
}