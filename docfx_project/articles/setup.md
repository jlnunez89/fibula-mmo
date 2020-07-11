# How to setup and run the service:

## 1) Clone using Git

1) Open your command prompt window with **Git** support installed.

1) Use **Git** to clone this repo:
    
    `git clone https://github.com/jlnunez89/fibula-mmo.git`

1) The resulting folder will contain this structure (or similar):

   ![Image of folder structure at repo root.](../images/folderstructure.PNG?raw=true)

## 2) Build the solution

1) You have some options to do this, but you'll need to:
   - Install .NetCore SDK 3.X from https://dotnet.microsoft.com/download.
   - Use Visual Studio (the included solution is for 2019 Community edition) 
   OR
   - Using a command line interface to build: 

        `dotnet build --configuration Release`

    ![Image of awesomeness, no errors, no warnings!](../images/hashtagnowarnings.PNG?raw=true)

> NOTE: You can run tests for the solution by running the command:
> 
>  `dotnet test`
> 
> That being said, there are not very many tests at the time of writing... help write some!
>  
> ![Image of some test results.](../images/someTestRun.PNG?raw=true)

1) After you build it, the project you want to focus on is `Fibula.Standalone`. Go into the bin folder created, and it should look something like this:

    ![Image of Fibula.Standalone/bin/Release/netcoreapp3.1 folder contents](../images/standaloneBinReleaseFolder.PNG?raw=true)

## 3) Extract the Map sector files.

> These take a lot of space uncompressed, so I only included them as a zipped archive.

1) Look for the file named `map.zip` in the main repository folder.

1) Decompress the archive into the `Fibula.Standalone/bin/Release/netcoreapp3.1` folder you just inspected:
   
   ![Image of Fibula.Standalone/bin/Release/netcoreapp3.1 folder with the Map folder](../images/standaloneBinFolderWithMap.PNG?raw=true)

## 4) Run the binary: 

Running the server is simple:

1) From within the `Fibula.Standalone/bin/Release/netcoreapp3.1` folder, execute dotnet on the standalone's DLL:

    `dotnet .\Fibula.Standalone.dll`

   ![Image of running service](../images/standaloneExecution.PNG?raw=true)

> This server engine is designed to only load the sector files surronding players as they approach them, making it start _**really fast**_ and requiring just a few memory- currently hovering around **~60 MB** so far. This will change of course, as we switch to a non in-memory database, add more assets loading and pre-checks.

What's that? You don't see [Verbose] stuff when you run yours? That's by design! Go to [configuration](configuration.md) after this if you want to fiddle with that kind of thing.

## 5) Log in:

1) Use you favorite `7.72**` client to connnect to `localhost` / `127.0.0.1` (per vanilla configuration). In my case, I'm just using OTLand's IP Changer with a vanilla client:

    ![Image of Tibia Loader](../images/ipChanger.PNG?raw=true)

> ** Or whatever version you're running! At the time of writing, 7.72 is the default version that's selected as the project builds.

1) Use the credentials for the In-Memory database:

|||
|--|--|
|Account Number:|1|
|Password:|1|

These are currently seeded during the in-memory database model creation `OnModelCreating` method: [Fibula.Data.InMemoryDatabase\FibulaInMemoryDatabaseContext.cs](../code/Fibula.Data.InMemoryDatabase.FibulaInMemoryDatabaseContext.html#Fibula_Data_InMemoryDatabase_FibulaInMemoryDatabaseContext_OnModelCreating_Microsoft_EntityFrameworkCore_ModelBuilder_)
