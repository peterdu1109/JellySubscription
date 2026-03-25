(function() {
    console.log("JellySubscription Premium Client Script v1.1.1 loaded.");

    document.addEventListener("viewshow", function (e) {
        var view = e.detail.view || e.target;
        var path = e.detail.path || window.location.hash;

        if (path.indexOf("mypreferences") > -1 || view.classList.contains("myPreferencesPage")) {
            injectSubscriptionTab(view);
        }
    });

    function injectSubscriptionTab(view) {
        var tabsNav = view.querySelector('.content-primary .tabs-nav');
        if (!tabsNav) return;

        if (view.querySelector('#subscriptionTabBtn')) return;

        var tabBtn = document.createElement('button');
        tabBtn.id = "subscriptionTabBtn";
        tabBtn.className = "emby-tab-button";
        tabBtn.setAttribute("data-index", "99");
        tabBtn.innerHTML = '<div class="emby-button-foreground" style="color:#E94057; font-weight:bold;">👑 Abonnement</div>';
        
        var scrollerContext = tabsNav.querySelector('.emby-tabs-slider') || tabsNav;
        scrollerContext.appendChild(tabBtn);

        tabBtn.addEventListener('click', function() {
            var container = view.querySelector('.content-primary > .view-settings-content') || view.querySelector('.content-primary');
            if (container) {
                container.innerHTML = `
                    <style>
                        .premium-sub-container {
                            background: rgba(20,20,30,0.8);
                            border: 1px solid rgba(255,255,255,0.1);
                            border-radius: 16px;
                            padding: 40px;
                            text-align: center;
                            max-width: 600px;
                            margin: 2em auto;
                            backdrop-filter: blur(20px);
                            box-shadow: 0 20px 40px rgba(0,0,0,0.5);
                            transition: transform 0.3s ease;
                        }
                        .premium-sub-container:hover { transform: translateY(-5px); }
                        .status-badge {
                            display: inline-block;
                            padding: 8px 16px;
                            border-radius: 20px;
                            font-weight: bold;
                            font-size: 1.2em;
                            margin: 15px 0;
                            text-transform: uppercase;
                            letter-spacing: 1px;
                        }
                        .status-active { background: rgba(46, 204, 113, 0.2); color: #2ecc71; border: 1px solid #2ecc71; }
                        .status-lifetime { background: rgba(155, 89, 182, 0.2); color: #9b59b6; border: 1px solid #9b59b6; box-shadow: 0 0 15px #9b59b6; }
                        .status-inactive { background: rgba(231, 76, 60, 0.2); color: #e74c3c; border: 1px solid #e74c3c; }
                        
                        .sub-btn {
                            background: linear-gradient(90deg, #E94057, #F27121);
                            color: white;
                            border: none;
                            padding: 12px 30px;
                            border-radius: 30px;
                            font-size: 1.1em;
                            font-weight: bold;
                            cursor: pointer;
                            margin-top: 25px;
                            text-decoration: none;
                            display: inline-block;
                            box-shadow: 0 5px 15px rgba(233, 64, 87, 0.4);
                            transition: all 0.3s ease;
                        }
                        .sub-btn:hover { filter: brightness(1.2); transform: scale(1.05); }
                    </style>

                    <div class="premium-sub-container">
                        <img src="https://raw.githubusercontent.com/jellyfin/jellyfin-ux/master/branding/SVG/icon-transparent.svg" width="80" style="margin-bottom:15px;" />
                        <h2 style="margin:0; font-size: 2em; letter-spacing:-0.5px;">Accès Premium</h2>
                        <p style="opacity: 0.7; margin-bottom:0;">Statut en direct de vos privilèges de lecture sur le serveur.</p>
                        
                        <div id="subscriptionStatusLoading" style="margin-top:20px; opacity:0.5;">Analyses des droits en cours...</div>
                        
                        <div id="subscriptionStatusContainer" style="display:none;">
                            <div class="status-badge" id="lblSubStatus">INCONNU</div>
                            
                            <p style="font-size: 1.1em; margin: 10px 0;">
                                Expiration : <strong id="lblSubExpiry" style="color:#fff;">-</strong>
                            </p>
                            
                            <a class="sub-btn" id="btnSubscribe" href="#" target="_blank">
                                <span class="material-icons" style="vertical-align: middle; font-size:1.2em; margin-right:5px;"></span>
                                Gérer mon abonnement
                            </a>
                        </div>
                    </div>
                `;

                ApiClient.ajax({
                    type: "GET",
                    url: ApiClient.getUrl("Subscription/Status")
                }).then(function(result) {
                    var statusEl = container.querySelector('#lblSubStatus');
                    var expiryEl = container.querySelector('#lblSubExpiry');
                    
                    if (result && result.Status === "Lifetime") {
                        statusEl.innerText = "🌟 LIFETIME (À VIE)";
                        statusEl.className = "status-badge status-lifetime";
                        expiryEl.innerText = "Accès illimité permanent";
                    } else if (result && result.Status === "Active") {
                        statusEl.innerText = "ACTIF";
                        statusEl.className = "status-badge status-active";
                        expiryEl.innerText = new Date(result.ExpiryDate).toLocaleDateString();
                    } else {
                        statusEl.innerText = "INACTIF";
                        statusEl.className = "status-badge status-inactive";
                        expiryEl.innerText = "Aucun abonnement valide";
                    }

                    container.querySelector('#subscriptionStatusLoading').style.display = 'none';
                    container.querySelector('#subscriptionStatusContainer').style.display = 'block';

                    ApiClient.getPluginConfiguration("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D").then(function(cfg) {
                        var baseUrl = cfg.WebhookUrl || "https://votre-site-de-paiement.com/checkout";
                        container.querySelector('#btnSubscribe').href = baseUrl + "?userId=" + Dashboard.getCurrentUserId();
                    });

                }).catch(function(err) {
                    container.querySelector('#subscriptionStatusLoading').innerText = "Erreur de chargement. Le plugin Backend ne répond pas.";
                });
            }
        });
    }
})();
