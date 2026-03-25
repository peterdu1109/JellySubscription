// JellySubscription Client Script
// Injecté globalement dans l'UI Web de Jellyfin.

(function() {
    console.log("JellySubscription Client Script loaded.");

    // Hook principal pour attendre que le DOM du WebClient soit prêt ou l'utilisateur navigue
    document.addEventListener("viewshow", function (e) {
        var view = e.detail.view || e.target;
        var path = e.detail.path || window.location.hash;

        // On cible la vue "MyPreferences" ou "Profile"
        if (path.indexOf("mypreferences") > -1 || view.classList.contains("myPreferencesPage")) {
            injectSubscriptionTab(view);
        }
    });

    function injectSubscriptionTab(view) {
        // Obtenir le menu de navigation s'il existe
        var tabsNav = view.querySelector('.content-primary .tabs-nav');
        if (!tabsNav) return;

        // Prévenir la double injection
        if (view.querySelector('#subscriptionTabBtn')) return;

        // Ajout d'un bouton d'onglet
        var tabBtn = document.createElement('button');
        tabBtn.id = "subscriptionTabBtn";
        tabBtn.className = "emby-tab-button";
        tabBtn.setAttribute("data-index", "99");
        tabBtn.innerHTML = '<div class="emby-button-foreground">Mon Abonnement</div>';
        
        // Ajout du bouton dans le DOM
        var scrollerContext = tabsNav.querySelector('.emby-tabs-slider') || tabsNav;
        scrollerContext.appendChild(tabBtn);

        // Au clic, afficher l'UI d'abonnement
        tabBtn.addEventListener('click', function() {
            var container = view.querySelector('.content-primary > .view-settings-content') || view.querySelector('.content-primary');
            if (container) {
                container.innerHTML = `
                    <div class="verticalSection">
                        <h2>Mon Abonnement Jellyfin</h2>
                        <div id="subscriptionStatusLoading">Chargement de votre statut...</div>
                        <div id="subscriptionStatusContainer" style="display:none; margin-top:1em;">
                            <p><strong>Statut actuel :</strong> <span id="lblSubStatus"></span></p>
                            <p><strong>Date d'expiration :</strong> <span id="lblSubExpiry"></span></p>
                            <br/>
                            <a is="emby-button" class="raised button-submit" id="btnSubscribe" href="#" target="_blank">
                                <span style="padding: 10px;">Gérer mon abonnement / Payer</span>
                            </a>
                        </div>
                    </div>
                `;

                // Appel API
                ApiClient.ajax({
                    type: "GET",
                    url: ApiClient.getUrl("Subscription/Status")
                }).then(function(result) {
                    var statusEl = container.querySelector('#lblSubStatus');
                    var expiryEl = container.querySelector('#lblSubExpiry');
                    
                    if (result && result.Status === "Active") {
                        statusEl.innerText = "Actif";
                        statusEl.style.color = "green";
                        expiryEl.innerText = new Date(result.ExpiryDate).toLocaleDateString();
                    } else {
                        statusEl.innerText = "Inactif ou Expiré";
                        statusEl.style.color = "red";
                        expiryEl.innerText = "-";
                    }

                    container.querySelector('#subscriptionStatusLoading').style.display = 'none';
                    container.querySelector('#subscriptionStatusContainer').style.display = 'block';

                    // L'URL de paiement contient l'userId pour la traçabilité
                    ApiClient.getPluginConfiguration("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D").then(function(cfg) {
                        var baseUrl = cfg.WebhookUrl || "https://votre-site-de-paiement.com/checkout";
                        container.querySelector('#btnSubscribe').href = baseUrl + "?userId=" + Dashboard.getCurrentUserId();
                    });

                }).catch(function(err) {
                    container.querySelector('#subscriptionStatusLoading').innerText = "Erreur de chargement de l'abonnement.";
                    console.error("JellySubscription Error:", err);
                });
            }
        });
    }
})();
