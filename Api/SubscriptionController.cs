using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JellySubscription.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JellySubscription.Api;

[ApiController]
[Route("Subscription")]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(ILogger<SubscriptionController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Renvoie le statut de l'utilisateur actuel.
    /// </summary>
    [HttpGet("Status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<SubscriptionInfo> GetStatus()
    {
        var config = Plugin.Instance!.Configuration;
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        if (config.UserSubscriptions.TryGetValue(userId, out var sub))
            return Ok(sub);

        return Ok(new SubscriptionInfo { UserId = userId, Status = "Inactive", ExpiryDate = DateTime.MinValue });
    }

    /// <summary>
    /// Permet à l'admin de forcer un statut.
    /// </summary>
    [HttpPost("Admin/Update")]
    [Authorize(Policy = "RequiresElevation")] // Réservé à l'Admin Jellyfin
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult UpdateStatus([FromBody] SubscriptionInfo updateRequest)
    {
        var config = Plugin.Instance!.Configuration;
        
        config.UserSubscriptions[updateRequest.UserId] = new SubscriptionInfo
        {
            UserId = updateRequest.UserId,
            Status = updateRequest.Status,
            ExpiryDate = updateRequest.ExpiryDate,
            TransactionId = updateRequest.TransactionId ?? "MANUAL_ACTIVATION"
        };

        Plugin.Instance.SaveConfiguration();
        _logger.LogInformation("Statut d'abonnement mis à jour manuellement pour l'utilisateur {UserId}.", updateRequest.UserId);
        return Ok(new { Message = "Abonnement mis à jour" });
    }

    /// <summary>
    /// Capture des événements de paiement externes (Stripe/PayPal)
    /// Le système de Webhook doit être sécurisé via un SecretKey.
    /// </summary>
    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook()
    {
        var config = Plugin.Instance!.Configuration;
        var requestSign = Request.Headers["X-Signature"].ToString();
        
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        if (!VerifySignature(body, requestSign, config.WebhookSecretKey))
        {
            _logger.LogWarning("Requête Webhook refusée (signature invalide).");
            return Unauthorized("Signature invalide");
        }

        // Implémentation du parsing du JSON selon le provider externe...
        _logger.LogInformation("Webhook de paiement reçu et validé avec succès.");
        
        return Ok();
    }

    private bool VerifySignature(string payload, string signature, string secret)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret)) return false;
        
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower(); // ou Base64 selon le provider
        
        return hashString == signature.ToLower();
    }
}
