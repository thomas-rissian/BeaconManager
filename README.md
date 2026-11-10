# BaliseManager

Application de supervision de balises industrielles.

## Technologies

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- HTML, CSS, JavaScript et jQuery
- WebSocket natif

Le frontend ne nécessite ni Node.js ni npm. jQuery est déjà présent localement
dans `wwwroot/lib/jquery/`.

## Prérequis

Installer :

- le SDK .NET 10 ;
- SQLite3 ;
- l'outil `dotnet-ef`.

Vérifier les installations :

```bash
dotnet --version
sqlite3 --version
dotnet ef --version
```

Si `dotnet-ef` n'est pas installé :

```bash
dotnet tool install --global dotnet-ef
```

Sous Linux Debian/Ubuntu, SQLite3 peut être installé avec :

```bash
sudo apt update
sudo apt install sqlite3
```

## Installation du projet

Depuis la racine du dépôt :

```bash
dotnet restore
dotnet build
```

## Initialiser la base SQLite

La connexion SQLite est configurée dans `appsettings.json` :

```text
database/balise-manager.db
```

Créer ou mettre à jour le schéma avec les migrations Entity Framework Core :

```bash
dotnet ef database update
```

Insérer les données de démonstration :

```bash
sqlite3 -bail database/balise-manager.db < database/seeds/BaliseManager_seed.sql
```

Le script de seed peut être exécuté plusieurs fois sans dupliquer les données
prévues pour le développement local.

## Démarrer l'application

Lancer l'application en HTTP :

```bash
dotnet run --launch-profile http
```

Ouvrir ensuite :

```text
http://localhost:5095/
```

Le profil HTTPS est également disponible :

```bash
dotnet run --launch-profile https
```

Adresses utiles en développement :

- Application : `http://localhost:5095/`
- Swagger UI : `http://localhost:5095/api/docs`
- OpenAPI : `http://localhost:5095/openapi/v1.json`
- WebSocket : `ws://localhost:5095/ws/events`

## Tests

Les tests utilisent xUnit et Moq. Ils utilisent des services mockés et
n'accèdent pas à la base SQLite.

```bash
dotnet test Tests/BaliseManager.Tests.csproj
```

## Réinitialiser la base locale

Pour repartir d'une base créée par les migrations :

```bash
rm -f database/balise-manager.db
dotnet ef database update
sqlite3 -bail database/balise-manager.db < database/seeds/BaliseManager_seed.sql
```

Ne pas supprimer les fichiers du dépôt sur un environnement partagé sans
vérifier les changements locaux au préalable.
