using System;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using Microsoft.Extensions.Logging;

namespace JellySubscription;

/// <summary>
/// Intercepteur principal au démarrage de la lecture de médias.
/// Bloque les flux si l'abonnement n'est pas actif.
/// </summary>
public class ServerEntryPoint : IServerEntryPoint
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<ServerEntryPoint> _logger;

    public ServerEntryPoint(ISessionManager sessionManager, ILogger<ServerEntryPoint> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    public Task RunAsync()
    {
        _logger.LogInformation("JellySubscription : Initialisation des règles d'accès sécurisées (PlaybackStarting).");
        _sessionManager.PlaybackStarting += OnPlaybackStarting;
        return Task.CompletedTask;
    }

    private void OnPlaybackStarting(object? sender, PlaybackProgressEventArgs e)
    {
        var config = Plugin.Instance?.Configuration;
        if (config == null || !config.IsSubscriptionModeEnabled) 
            return;

        // Obtenir l'utilisateur demandant la lecture
        var userIdString = e.Users.FirstOrDefault()?.Id.ToString("N");
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) 
            return;

        // FEATURE : Validation Whitelist (Exclut spécifiquement des utilisateurs du Paywall)
        if (!string.IsNullOrEmpty(config.WhitelistedUserIds) && config.WhitelistedUserIds.Contains(userIdString, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("JellySubscription : Accès autorisé par Whitelist pour l'utilisateur {UserId}", userId);
            return;
        }

        bool hasAccess = false;
        if (config.UserSubscriptions.TryGetValue(userId, out var sub))
        {
            // FEATURE : Période de grâce (ajoute X jours à la validation avant de bloquer)
            var validUntil = sub.ExpiryDate.AddDays(config.GracePeriodDays);
            hasAccess = sub.Status == "Active" && validUntil > DateTime.UtcNow;
        }

        if (!hasAccess)
        {
            _logger.LogWarning("JellySubscription : Lecture bloquée pour l'utilisateur {UserId}. Abonnement invalide.", userId);
            
            // Notification visuelle de l'utilisateur si Jellyfin a une session WebSocket ouverte
            _sessionManager.SendMessageCommand(e.Session.Id, e.Session.Id, new MediaBrowser.Model.Session.MessageCommand
            {
                Header = "Accès Refusé",
                Text = config.CustomBlockedMessage
            }, new System.Threading.CancellationToken());

            // Lève une exception silencieuse pour stopper la chaîne de lecture (compatible 10.11+)
            throw new ArgumentException(config.CustomBlockedMessage);
        }
    }

    public void Dispose()
    {
        if (_sessionManager != null)
        {
            _sessionManager.PlaybackStarting -= OnPlaybackStarting;
        }
    }
}
