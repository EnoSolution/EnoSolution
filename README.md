# EnoSolution

EnoSolution est une suite domotique basée sur le protocole EnOcean.
- EnoSolution est un projet libre, placé sous licence GPL, et développé en C# sous Visual Studio 2017 Community.
- EnOcean est un protocole domotique ouvert, documenté, normalisé ISO, et supporté par de nombreux constructeurs de capteurs et actionneurs.

Dans les grandes lignes, le projet EnoSolution est architecturé autour d'un service Windows nommé EnoGateway qui assure le rôle de passerelle entre le réseau radio EnOcean et le réseau local. Les autres modules de la suite utilisent EnoGateway pour dialoguer avec le réseau EnOcean. Chaque module peut ainsi remplir sa propre fonction (configuration, automatisme, IHM, outils de mise au point ou d'aide à l'installation).

Cette architecture est dictée par le fait que le dialogue avec un réseau EnOcean passe forcément par un dongle USB qui intégre un émetteur/récepteur radio EnOcean. Mais ce dongle est vu par Windows comme un port série, qui ne peut donc être ouvert que par une seule application. Pour éviter cette contrainte, il faut disposer d'une passerelle IP matérielle ou logicielle entre le port série sur lequel est connecté le dongle et le réseau IP local. C'est le rôle de la passerelle IP logicielle EnoGateway.

EnoGateway reçoit les télégrammes via un dongle USB EnOcean (type USB300 ou USB310) et les réémet en UDP multicast afin de permettre à plusieurs applications de s'y abonner. Dans l'autre sens, EnoGateway écoute les télégrammes émis en UDP par les aplications, puis calcule le BASE_ID et les CRC avant d'émettre les télégrammes sur le réseau radio via le dongle.

EnoGateway est aussi la seule application de la suite EnoSolution qui peut être utilisée seule, par un développeur qui souhaiterait se lancer dans la programmation d'applications EnOcean, tout en minimisant l'investissement logiciel initial, car EnoGatway :
- permet aux applications qui reçoivent de s'affranchir du contrôle de la validité des télégrammes reçus.
- permet aux applications qui émettent de ne pas se préocupper du calcul des CRC (il est réalisé par EnoGateway).
- permet aux applications qui émettent de ne pas avoir à renseigner l'identifiant BASE_ID du dongle (il suffit de préciser un identifiant relatif de l'émetteur compris entre 0 et 127).

D'autres applicatons viendront compléter EnoGateway pour composer une véritable suite logicielle. Parmi les applications envisagées ou en cours de développement, on trouve :
- EnoViewer : Une application dédiée à la mise au point permettant de lister au fil de l'eau les télégrammes qui transitent sur le réseau EnOcean.
- EnoLearn : Une application facilitant l'appairage entre les actionneurs de l'installation et les capteurs émulés par les applications (via les 128 identifiants dont dispose le dongle pour émettre).
- EnoConfig : Une application permettant de définir les capteurs de l'installation (boutons poussoirs, capteurs de température, d'hygrométrie, de luminosité, station météo, etc, en y associant notamment un libellé, un profil d'équipement EnOcean (EEP), et les paramètres permettant de convertir les valeurs brutes reçues en valeurs exprimées dans des unités exploitables.
- EnoDecision : Une application de type service Windows dédié à l'exécution des automatismes de l'installation, comme par exemple la fermeture des volets via une horloge astronomique, la remontée des stores par vent trop fort, la fermeture des lames de la pergola bioclimatique sur détection de pluie, etc. La configuration de ces automatismes sera réalisée par EnoConfig.
- EnoControl : Une application de type IHM (interface Homme Machine) destinée à tourner sur PC ou tablette murale fonctionnant sous Windows. Cette application permettra d'avoir une vue d'ensemble graphique de l'installation, et de lancer des commandes ou des séquences de commande d'un simple clic sur un objet.

Le projet est en cours de développement et n'en est encore qu'à ses débuts. Les modules et la documentation seront mis en ligne au fur et à mesure, à commencer par EnoGateway.

Soyez patients. Vous savez aussi bien que moi que c'est toujours le temps qui fait le plus défaut à un développeur...

Enos
