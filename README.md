### A C# NetCore OpenTibia server, for client version 7.71.

# Frequently Asked Questions
## Why C#/NetCore?
Because it is **awesome**. It is fast, simple and way more maintainable than C++. 
It's easier for newbies to pick up and learn, as it is often taught in schools and bootcamps.

It's also what half the industry uses and, well, I happen to be part of that half of the industry. 

_And lastly, yeah- generally whoever starts coding something from scratch gets to pick..._

## Why choose version 7.71?
There was an infamous CipSoft breach back on 2007 in which all files and assets required to run the original game at the time were leaked, finally leaked to . 

Having access to the original game files and assets simplifies our goal to mimic and reverse engineer the original game, leaving us with an implementation that is as closest to the real game as it gets. 

On a more personal note, I hate aimbots (and hotkeys) since they ruin the game. Aimbots are fairly easy to catch with simple metric analysis and automation, and hotkeys were introduced on shortly after this version (7.8, or 8.0?) of the game.

I'm not however, opposed to develop any other versions of the game later on, but I'm super fond of 7.x versions as these were the ones in which I actually played the real game.

##### I'll add more FAQ as they come along...


# How to setup and run

### Clone using Git

1) Open your command window with Git support installed.
1) Use Git [0] to clone this repo:
    
    `git clone https://github.com/jlnunez89/opentibiacore.git`

1) The resulting folder will contain this structure (or similar):

   ![Image of folder structure](docs/readme/folderstructure.PNG?raw=true)

### Build the solution

You have several options to do this:
- Use Visual Studio (the included solution is for 2019 Community edition)
- Using command line interface: 

    `msbuild .\OpenTibia.sln`

After you build it, the project you want to focus on is `OpenTibia.Server.Standalone`. Go into the bin folder created, and it should look something like this:

   ![Image of Bin folder structure](docs/readme/standaloneBinFolder.PNG?raw=true)

### Extract the Map sector files.

> These take a lot of space uncompressed, so I only included them as a zipped archive.

1) Look for the file named `map.zip` in the main repository folder.

2) Decompress the archive into the `OpenTibia.Server.Standalone` folder:
   
   ![Image of Bin folder structure with Map folder](docs/readme/standaloneBinFolderWithMap.PNG?raw=true)

### Run the binary: 

Running the server is simple. From the `OpenTibia.Server.Standalone`'s bin folder, execute dotnet on the standalone's DLL:

`dotnet .\OpenTibia.Server.Standalone.dll`

   ![Image of running service](docs/readme/standaloneExecution.PNG?raw=true)

### Log in:

Use you favorite `7.7` (soon to be 7.71) client to connnect to `localhost` / `127.0.0.1` (per vanilla configuration). In my case, I use Tibia Loader:

   ![Image of Tibia Loader](docs/readme/tibiaLoader.PNG?raw=true)

Credentials for the In-Memory database are currently:

``` 
Account Number:     1
Password:           1
```

> These are currently seeded on the database model creation method, under
> `OpenTibia.Data.InMemoryDatabase\OpenTibiaInMemoryDatabaseContext.cs`

### Service configuration:

Configuration files inclue:

- Application Settings file: `appsettings.json`

    > Note: Not all of the settings are currently in use, but left there as preparation.

    All the settings here should be pretty self-explanatory, except maybe for the `KeyVaultSecretsProviderOptions` and `CosmosDbConfigurationOptions` which you may remove if you're not using Azure CosmosDB as the database store.

    I am currently using it instead of the local In-Memory database, and because it uses the Azure KeyVault to store secrets, such as the CosmosDB connection string, I'm including it on the standard configuration.

- Log Settings file: `logsettings.json`
  
  To control how verbose you want the log events to show on the console, change the values you see on the file to:
  
    `Verbose | Debug | Information | Warning | Error`

    for example: You can set the `Console` sink's `restrictedToMinimumLevel` to `Information` to see less messages output:
  ```
    {
    "Serilog": {
        "MinimumLevel": "Verbose",
        ...
        "WriteTo": [
            {
                "Name": "Console",
                "Args": {
                "restrictedToMinimumLevel": "Debug",
                "outputTemplate": "[{Timestamp:HH:mm:ss}] [{Level}] {SourceContext}: {Message}{NewLine}{Exception}"
                }
            },
        ]
        ...
    }
  ```
  

