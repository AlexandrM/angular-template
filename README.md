# AngularTemplate

**Layers**
Frontend <-> Backend <-> Services <-> Repositories <-> Database

*1. AngularTemplate.SPA*
Frontend layer
Angular

*2. AngularTemplate.Web*
Backend layer
ASP Core

*3. AngularTemplate.Data.Services*
Services layer

*5. AngularTemplate.Data.Models*
Models

*6. AngularTemplate.Data.Repositories*
Database layer

*7. MyOffice.DbContext*
AppDbContext
EntityFramework Core

*8. MyOffice.Migrations.Postgres*
Postgres Migrations

*9. MyOffice.Migrations.Sqlite*
Sqlite Migrations


**DataBase**
1. SQLite
2. PostgreSql
3. MS SQL

**Develompent**
1. Local IIS
2. Express IIS

**Authentication & Authorization**

1. Identity Server 4
2. Custom UserStore
3. Login with Google
4. Login with Auth0

**Build & Deploy**
1. Build with .bat 
2. Deploy to Linux

**Get started**
1. create new project
*dotnet new angular-template.ase.com.ua -p MyProject*
2. Build (restore nuget and npm packages)
3. Run Frontend, in Terminal ".\spa_start.bat" (default http://localhost:4200)
2. Set startup project to MyProject.Web
5. Select 'IIS Express', Run Backend (F5)
6. open in browser http://localhost:4200

**add site to IIS**
1. add
Site name:myproject
Physical path:...\MyProject\MyProject.Web
Port:9100
2. Select 'IIS', Run Backend (F5)

**Settings**
1. launchSettings.json
2. appsettings
appsettings.json
appsettings.Development.json
appsettings.Production.json

**External providers**
1. Auth0
appsettings ExternalProviders:Auth0
2. Google
appsettings ExternalProviders:Google

**PowerShell scripts**
1. use 'Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass' to allow run *.ps1
2. params.txt
2.1. publish_folder=_Published (folder to publish)
2.2. linux_user=root (linux user)
2.3. linux_host=192.168.1.215 (linux host)
2.4. linux_folder=/var/www/MyProject (folder in linux)

**Build**
1. .\build.ps1
1.1. '.\build.ps1' - build Backend + Frontend
1.2. '.\build.ps1 dotnet' build Backend
1.3. '.\build.ps1 spa' build Frontend

**Deploy to linux**
1. linux_deploy.ps1
1.1. '.\linux_deploy.ps1' - copy to linux and run
1.2. '.\linux_deploy.ps1 copy' copy to linux
1.3. '.\linux_deploy.ps1 run' run at linux
