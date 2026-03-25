using MediaBrowser.Model.Plugins;
using System;
using System.Collections.Generic;

namespace JellySubscription.Configuration;

/// <summary>
/// Configuration centralisée du plugin JellySubscription.
/// Sérialisée automatiquement par Jellyfin au format XML.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    // ----- PARAMETRES GLOBAUX -----
    public bool IsSubscriptionModeEnabled { get; set; }
    public decimal SubscriptionPrice { get; set; }
    public string WebhookUrl { get; set; }
    public string WebhookSecretKey { get; set; }
    
    // ----- FONCTIONNALITES ADMIN (Nouvelles Features) -----
    public int GracePeriodDays { get; set; }
    public string WhitelistedUserIds { get; set; }
    public string CustomBlockedMessage { get; set; }

    // ----- BASE DE DONNÉES UTILISATEURS -----
    public Dictionary<Guid, SubscriptionInfo> UserSubscriptions { get; set; }

    public PluginConfiguration()
    {
        IsSubscriptionModeEnabled = false;
        SubscriptionPrice = 5.00m;
        WebhookUrl = string.Empty;
        WebhookSecretKey = Guid.NewGuid().ToString("N");
        
        // Valeurs par défaut des nouvelles options
        GracePeriodDays = 0;
        WhitelistedUserIds = string.Empty;
        CustomBlockedMessage = "Abonnement requis. Veuillez renouveler votre accès depuis votre profil.";
        
        UserSubscriptions = new Dictionary<Guid, SubscriptionInfo>();
    }
}

public class SubscriptionInfo
{
    public Guid UserId { get; set; }
    public string Status { get; set; } // e.g., "Active", "Inactive"
    public DateTime ExpiryDate { get; set; }
    public string TransactionId { get; set; }
}
