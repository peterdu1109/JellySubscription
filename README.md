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

## 🚀 Installation Rapide

> **Prérequis :** Jellyfin Serveur 10.11.x ou supérieur.

1. Téléchargez la dernière [Release](https://github.com/peterdu1109/JellySubscription/releases) compilée (`JellySubscription_net9.zip`).
2. Extrayez le fichier `.dll` dans le dossier `plugins/JellySubscription` de votre serveur.
3. Redémarrez Jellyfin.
4. Rendez-vous dans **Tableau de Bord > Plugins > JellySubscription** pour paramétrer vos prix, clés secrètes et périodes de grâce.

## ⚙️ Configuration du Webhook (Développeurs)

Pour valider automatiquement un abonnement, configurez votre fournisseur de paiement pour envoyer une requête `POST` vers le serveur Jellyfin :
`POST https://<votre-serveur-jellyfin.com>/Subscription/Webhook`

Incluez obligatoirement le header de signature : `X-Signature: <votre-hash-HMAC-SHA256>`. Le corps de la requête doit idéalement contenir l'ID du paiement pour le connecter au profil Jellyfin.

<hr/>

<div align="center">
  Créé avec passion par <a href="https://github.com/peterdu1109">peterdu1109</a> & l'équipe Jellypay.
</div>
