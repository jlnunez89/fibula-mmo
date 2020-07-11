# Service configuration:

Currently, the configuration files are:
`appsettings.json` and `logsettings.json`

## Application Settings file: `appsettings.json`

> ## Note: Not all of the settings are currently in use, but left there as preparation.

All the settings here should be pretty self-explanatory, except maybe for the `KeyVaultSecretsProviderOptions` and `CosmosDbConfigurationOptions` which you may remove if you're not using Azure CosmosDB as the database store.

I am currently using it instead of the local In-Memory database, and because it uses the Azure KeyVault to store secrets, such as the CosmosDB connection string, I'm including it on the standard configuration.

## Log Settings file: `logsettings.json`
  
To control how verbose you want the log events to show on the console, change the values you see on the file to:
  
`Verbose | Debug | Information | Warning | Error`

For example: You can set the `Console` sink's `restrictedToMinimumLevel` to `Information` to see less messages output:

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
}
```
  

