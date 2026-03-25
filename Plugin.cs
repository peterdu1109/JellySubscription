using System;
using System.Collections.Generic;
using JellySubscription.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace JellySubscription;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public override string Name => "JellySubscription";
    public override Guid Id => Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D");
    public override string Description => "Système d'abonnement obligatoire pour accéder au contenu du serveur.";

    public static Plugin? Instance { get; private set; }

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            // Dashboard Admin Page
            new PluginPageInfo
            {
                Name = this.Name,
                EmbeddedResourcePath = string.Format("{0}.Web.adminPage.html", GetType().Namespace),
                IsPluginConfigurationPage = true
            },
            // Client Script Injection for User Profile
            new PluginPageInfo
            {
                Name = "JellySubscriptionProfileScript",
                EmbeddedResourcePath = string.Format("{0}.Web.subscription-profile.js", GetType().Namespace)
            }
        };
    }
}
