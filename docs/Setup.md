# How to setup and run the service:

## 1) Clone using Git

1) Open your command prompt window with **Git** support installed.

1) Use **Git** to clone this repo:
    
    `git clone https://github.com/jlnunez89/opentibiacore.git`

1) The resulting folder will contain this structure (or similar):

   ![Image of folder structure](readme/folderstructure.PNG?raw=true)

## 2) Build the solution

1) You have some options to do this, but you'll need to:
   - Install .NetCore SDK 3.X from https://dotnet.microsoft.com/download.
   - Use Visual Studio (the included solution is for 2019 Community edition) 
   OR
   - Using a command line interface to build: 

        `dotnet build --configuration Release`

1) After you build it, the project you want to focus on is `OpenTibia.Server.Standalone`. Go into the bin folder created, and it should look something like this:

    ![Image of Bin folder structure](readme/standaloneBinFolder.PNG?raw=true)

## 3) Extract the Map sector files.

> These take a lot of space uncompressed, so I only included them as a zipped archive.

1) Look for the file named `map.zip` in the main repository folder.

1) Decompress the archive into the `OpenTibia.Server.Standalone` folder:
   
   ![Image of Bin folder structure with Map folder](readme/standaloneBinFolderWithMap.PNG?raw=true)

## 4) Run the binary: 

Running the server is simple:

1) From the `OpenTibia.Server.Standalone`'s bin folder, execute dotnet on the standalone's DLL:

    `dotnet .\OpenTibia.Server.Standalone.dll`

   ![Image of running service](readme/standaloneExecution.PNG?raw=true)

> This server is designed to only load the sector files surronding players as they approach them, making it start _**really fast**_ and requiring just a few memory: usually hovering around **250kb** so far. This will change of course as we add up monsters, their behaviour and NPCs.

## 5) Log in:

1) Use you favorite `7.7` (soon to be 7.71) client to connnect to `localhost` / `127.0.0.1` (per vanilla configuration). In my case, I use Tibia Loader:

    ![Image of Tibia Loader](readme/tibiaLoader.PNG?raw=true)

1) Use the credentials for the In-Memory database:

``` 
Account Number:     1
Password:           1
```

> NOTE: These are currently seeded on the database model creation method, under  `OpenTibia.Data.InMemoryDatabase\OpenTibiaInMemoryDatabaseContext.cs`
