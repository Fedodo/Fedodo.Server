using System.Security.Cryptography;
using System.Text;
using ActivityPubServer.Model.ActivityPub;
using Microsoft.AspNetCore.Mvc;

namespace ActivityPubServer.Controllers;

[Route("Inbox")]
public class InboxController : ControllerBase
{
    private readonly ILogger<InboxController> _logger;

    public InboxController(ILogger<InboxController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> GeneralInbox([FromBody] Activity activity)
    {
        _logger.LogTrace($"Entered {nameof(GeneralInbox)} in {nameof(InboxController)}");

        if (!await VerifySignature(HttpContext.Request.Headers, "/inbox")) return BadRequest("Invalid Signature");

        return Ok();
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> Log(Guid userId, [FromBody] Activity activity)
    {
        _logger.LogTrace($"Entered {nameof(Log)} in {nameof(InboxController)}");

        if (!await VerifySignature(HttpContext.Request.Headers, $"/inbox/{userId}")) return BadRequest("Invalid Signature");


        return Ok();
    }

    private async Task<bool> VerifySignature(IHeaderDictionary requestHeaders, string currentPath)
    {
        _logger.LogTrace("Verifying Signature");

        var signatureHeader = requestHeaders["Signature"].First().Split(",").ToList();

        foreach (var item in signatureHeader)
        {
            _logger.LogDebug($"Signature Header Part=\"{item}\"");
        }
        
        var keyId = new Uri(signatureHeader.FirstOrDefault(i => i.StartsWith("keyId"))?.Replace("keyId=", "")
            .Replace("\"", "").Replace("#main-key", "") ?? string.Empty);
        var signatureHash = signatureHeader.FirstOrDefault(i => i.StartsWith("signature"))?.Replace("signature=", "")
            .Replace("\"", "");
        _logger.LogDebug($"KeyId=\"{keyId}\"");

        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Accept", "application/ld+json");
        var response = await http.GetAsync(keyId);
        if (response.IsSuccessStatusCode)
        {
            var resultActor = await response.Content.ReadFromJsonAsync<Actor>();

            var rsa = RSA.Create();
            rsa.ImportFromPem(resultActor.PublicKey.PublicKeyPem.ToCharArray());

            var comparisionString =
                $"(request-target): post {currentPath}\nhost: {requestHeaders.Host}\ndate: {requestHeaders.Date}\ndigest: {requestHeaders["Digest"]}"; // TODO Recompute Digest from Body TODO Validate Time
            _logger.LogDebug($"{nameof(comparisionString)}=\"{comparisionString}\"");
            if (rsa.VerifyData(Encoding.UTF8.GetBytes(comparisionString), Convert.FromBase64String(signatureHash),
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            {
                _logger.LogDebug("Action with valid Signature received.");
                return true;
            }

            _logger.LogWarning("Action with invalid Signature received!!!");
            return false;
        }

        _logger.LogInformation("Could not retrieve PublicKey");
        return false;
    }
}