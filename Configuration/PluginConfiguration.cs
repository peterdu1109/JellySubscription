using MediaBrowser.Model.Plugins;
using System;
using System.Collections.Generic;

namespace JellySubscription.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool IsSubscriptionModeEnabled { get; set; }
    public decimal SubscriptionPrice { get; set; }
    public string WebhookUrl { get; set; }
    public string WebhookSecretKey { get; set; }
    public Dictionary<Guid, SubscriptionInfo> UserSubscriptions { get; set; }

    public PluginConfiguration()
    {
        IsSubscriptionModeEnabled = false;
        SubscriptionPrice = 5.00m;
        WebhookUrl = string.Empty;
        WebhookSecretKey = Guid.NewGuid().ToString("N");
        UserSubscriptions = new Dictionary<Guid, SubscriptionInfo>();
    }
}

public class SubscriptionInfo
{
    public Guid UserId { get; set; }
    public string Status { get; set; } // "Active", "Inactive"
    public DateTime ExpiryDate { get; set; }
    public string TransactionId { get; set; }
}
