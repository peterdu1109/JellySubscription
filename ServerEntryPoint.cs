using System;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Session;
using Microsoft.Extensions.Logging;

namespace JellySubscription;

public class ServerEntryPoint : Microsoft.Extensions.Hosting.IHostedService
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<ServerEntryPoint> _logger;

    public ServerEntryPoint(ISessionManager sessionManager, ILogger<ServerEntryPoint> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    public Task StartAsync(System.Threading.CancellationToken cancellationToken)
    {
        _logger.LogInformation("JellySubscription : Initialisation des règles d'accès sécurisées (SessionStarted).");
        _sessionManager.SessionStarted += OnSessionStarted;
        return Task.CompletedTask;
    }

    private void OnSessionStarted(object? sender, SessionEventArgs e)
    {
        var config = Plugin.Instance?.Configuration;
        if (config == null || !config.IsSubscriptionModeEnabled) 
            return;

        var userIdString = e.SessionInfo?.UserId.ToString("N");
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId) || userId == Guid.Empty) 
            return;

        if (!string.IsNullOrEmpty(config.WhitelistedUserIds) && config.WhitelistedUserIds.Contains(userIdString, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("JellySubscription : Accès autorisé par Whitelist pour l'utilisateur {UserId}", userId);
            return;
        }
        
        // FEATURE v1.1.1 : On tente de détecter si la tentative de lecture cible un dossier ignoré
        // Note: Dans SessionStarted, le média spécifique n'est pas encore chargé (seulement au PlaybackStarting).
        // Le blocage global de session est maintenu, mais s'il a le statut Lifetime, il passe.

        bool hasAccess = false;
        if (config.UserSubscriptions.TryGetValue(userId, out var sub))
        {
            if (sub.Status == "Lifetime") 
            {
                hasAccess = true;
            }
            else 
            {
                var validUntil = sub.ExpiryDate.AddDays(config.GracePeriodDays);
                hasAccess = sub.Status == "Active" && validUntil > DateTime.UtcNow;
            }
        }

        if (!hasAccess)
        {
            _logger.LogWarning("JellySubscription : Session bloquée pour l'utilisateur {UserId}. Abonnement invalide.", userId);
            
            _sessionManager.SendMessageCommand(e.SessionInfo.Id, e.SessionInfo.Id, new MediaBrowser.Model.Session.MessageCommand
            {
                Header = "Accès Refusé",
                Text = config.CustomBlockedMessage
            }, new System.Threading.CancellationToken());

            try {
                _sessionManager.Logout(e.SessionInfo.Id);
            } catch {
                _logger.LogError("JellySubscription : Impossible de déconnecter l'utilisateur.");
            }
        }
    }

    public Task StopAsync(System.Threading.CancellationToken cancellationToken)
    {
        if (_sessionManager != null)
        {
            _sessionManager.SessionStarted -= OnSessionStarted;
        }
        return Task.CompletedTask;
    }
}
