<div align="center">
  <img src="https://raw.githubusercontent.com/jellyfin/jellyfin-ux/master/branding/SVG/icon-transparent.svg" alt="Jellyfin Logo" width="100" />
  <h1>JellySubscription</h1>

  <p>
    <strong>Le premier système complet d'abonnement et de paywall pour Jellyfin (10.11+)</strong>
  </p>

  [![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=fff)](#)
  [![Jellyfin](https://img.shields.io/badge/Jellyfin-10.11-00A4DC?logo=jellyfin&logoColor=fff)](#)
  [![License](https://img.shields.io/badge/License-MIT-green.svg)](#)
</div>

<hr/>

## ✨ Fait pour les Créateurs
Transformez votre serveur Jellyfin en un service premium. **JellySubscription** restreint automatiquement le contenu multimédia (vidéos, musiques) aux seuls utilisateurs possédant un abonnement actif.

### 🌟 Fonctionnalités Modernes
- 🔒 **Paywall Intégré :** Bloque intelligemment la lecture des flux médias via l'interception native au niveau du Serveur.
- 💳 **Intégration Webhook :** Capturez automatiquement les validations de paiement depuis Stripe, PayPal, ou toute autre plateforme via une `SecretKey`.
- 🛡️ **Période de Grâce & Whitelist :** Flexibilité totale pour autoriser les administrateurs ou accorder quelques jours supplémentaires avant la coupure.
- 🎨 **Interface Native :** Ajoute une section *"Mon Abonnement"* transparente directement dans les préférences de l'utilisateur Jellyfin.
- 💼 **Dashboard Admin Complet :** Activez, désactivez ou forcez les abonnements depuis le panneau de configuration du serveur.

## 🚀 Installation

> **Prérequis :** Serveur Jellyfin version **10.11.x ou supérieur**.

### Méthode 1 : Via les Dépôts Jellyfin (Recommandé)
Bénéficiez des mises à jour automatiques en ajoutant ce dépôt à votre serveur.
1. Allez dans **Tableau de Bord** > **Dépôts (Repositories)**.
2. Ajoutez un nouveau dépôt en entrant :
   - **Nom :** JellySubscription
   - **URL :** `https://raw.githubusercontent.com/peterdu1109/JellySubscription/main/manifest.json`
3. Allez dans l'onglet **Catalogue** et cherchez `JellySubscription`.
4. Installez le plugin et redémarrez Jellyfin.

### Méthode 2 : Installation Manuelle
1. Téléchargez la dernière [Release Zip](https://github.com/peterdu1109/JellySubscription/releases).
2. Procédez à l'extraction dans le dossier `plugins/JellySubscription` de votre serveur.
3. Redémarrez Jellyfin.

Une fois installé, rendez-vous dans le menu **Plugins** pour paramétrer la période de grâce, la clé de sécurité et vos utilisateurs autorisés !

## ⚙️ Configuration du Webhook (Développeurs)

Pour valider automatiquement un abonnement, configurez votre fournisseur de paiement pour envoyer une requête `POST` vers le serveur Jellyfin :
`POST https://<votre-serveur-jellyfin.com>/Subscription/Webhook`

Incluez obligatoirement le header de signature : `X-Signature: <votre-hash-HMAC-SHA256>`. Le corps de la requête doit idéalement contenir l'ID du paiement pour le connecter au profil Jellyfin.

<hr/>

<div align="center">
  Créé avec passion par <a href="https://github.com/peterdu1109">peterdu1109</a> & l'équipe Jellypay.
</div>
