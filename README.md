# EnoSolution

English translation at the end of the document.

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


EnoSolution is a home automation suite based on the EnOcean protocol.
- EnoSolution is a free project, licensed under the GPL, and developed in C # under Visual Studio 2017 Community.
- EnOcean is an open, documented, standardized ISO protocol, supported by many sensor and actuator manufacturers.

Broadly speaking, the EnoSolution project is built around a Windows service called EnoGateway that acts as a gateway between the EnOcean radio network and the local network. The other modules in the suite use EnoGateway to interact with the EnOcean network. Each module can thus fulfill its own function (configuration, automation, HMI, debugging tools or installation aid).

This architecture is dictated by the fact that the dialogue with an EnOcean network necessarily passes through a USB dongle that integrates an EnOcean radio transmitter / receiver. But this dongle is seen by Windows as a serial port, which can therefore only be opened by a single application. To avoid this constraint, you must have a hardware or software IP gateway between the serial port on which the dongle is connected and the local IP network. This is the role of EnoGateway.

EnoGateway receives the telegrams via an EnOcean USB dongle (USB300 or USB310) and re-transmits them in UDP multicast to allow several applications to subscribe. In the other direction, EnoGateway listens to the telegrams transmitted in UDP by the applications and then calculates the BASE_ID and the CRCs before sending the telegrams on the radio network via the dongle.

EnoGateway is also the only application in the EnoSolution suite that can be used on its own by a developer who would like to start programming EnOcean applications while minimizing the initial software investment because EnoGatway:
- Allows the applications that receive to dispense with the control of the validity of the received telegrams.
- Allows the applications that emit not to worry about CRC calculation (it is done by EnoGateway).
- Allows the emitting applications not to have to enter the BASE_ID of the dongle (you need only to specify a relative identifier of the transmitter between 0 and 127).

Other applications will complement EnoGateway to create a real software suite. Among the applications envisaged or under development are:
- EnoViewer: A dedicated debugging application for displaying telegrams transiting the EnOcean network.
- EnoLearn: An application that facilitates the pairing between the actuators of the installation and the sensors emulated by the applications (via the 128 identifiers available to the dongle to transmit).
- EnoConfig: An application to define the sensors of the installation (pushbuttons, sensors of temperature, humidity, luminosity, weather station, etc, including equipment name, EnOcean equipment profile, And the parameters for converting the received raw values into values expressed in exploitable units.
- EnoDecision: An application of the Windows service type dedicated to the execution of the automation of the installation, such as closing of the shutters via an astronomical clock, the rise of the blinds in wind too strong, the closing of the blades of the bioclimatic pergola on Rain detection, etc. The configuration of these automations will be carried out by EnoConfig.
- EnoControl: An HMI software intended to run on a PC or a wall digital tablet running Windows. This application will allow you to have a graphical overview of the installation, and to launch commands or sequences of command with a simple click on an object.

The project is under development and is still in its infancy. The modules and documentation will be delivered as you go, starting with EnoGateway.

Be patient. You know as well as I that it is always the time that is most lacking to a developer ...

Enos
