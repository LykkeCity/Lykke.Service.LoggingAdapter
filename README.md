# Lykke.Service.LoggingAdapter
Lightweight adapter of Lykke logging system

## What this service is inteded for
Main purpose of the service - to provide access to lykke logging infrastucture using http or wamp api.

## What environment is required to run the service
You need to install .NET Core runtime (https://www.microsoft.com/net/download) on your machine.

## How to build and run the service
To run service in production mode you need provide access to json settings. The apps should read settings from the URL or local file specified in the SettingsUrl environment variable.

## Endpont Specification

Service exposes OpenAPI specification, based on swagger (https://swagger.io/)
Check the /swagger/ui/index.html# url to try it yourself.

POST /api/logs
Endpoint accepts single log entry with next parameters
* appName: string (required)
* appVersion: string (required)
* envInfo: string (optional)
* level {info, warning, monitor, error, fatalerror} (required)
* component: string (optional, appName should be used, if omited)
* process: string (optional)
* context: string (optional)
* message: string (required for levels {info, warning, monitor}, optional for levels {error, fatalerror})
* callstack: string (required for levels {error, fatalerror}, optional for level warning)
* exceptionType: string (required for levels {error, fatalerror}, optional for level warning)
* additionalSlackChannels: string[] (optional)
