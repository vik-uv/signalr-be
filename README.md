# Notification service
The main goal of this project is an investigation of possibilities of data exchange using sockets and Azure serverless services. So, it's possible tp have some functional bugs in the current implementation, I tried to understand how all these technology stack worked.

On the functional level, the project presents a simple function to add records to a CosmosDB collection, listen changes, and show them on a form.

Tne project is base on:
- **Back-end:** Azure functions (.NET C#), SignalR, API Management, CosmosDB, Static Web site on Storage
- **Front-end:** Angular 14, PrimeNG components, SignalR

I can illustrate the architecture in this picture

![Application architecture](https://notificationapptest.z20.web.core.windows.net/notification-arch.png)

The project is available [online](https://notificationapptest.z20.web.core.windows.net/)
- login: testuser
- password: 73M2EStxAo5w

You can add a message using the same browser and check that the messaged will be appeared in the list.

The latency is big (usually ~2 seconds), but it can be related to weak Azure resources that I use for this test project.

## Front-end/Back-end communication
It's very nice to get the TypeScript implementation for API and models, based on the existing API on BE. There are many projects that provide this feature and I choose [NSwag tool](https://github.com/RicoSuter/NSwag) to generate TypeScript code for the Angular part. It can get API directly from .NET project but I decide to generate the code from swagger.json file, as more universal solution. We still need to fix the resulting *.ts file, but I hope that it will be fixed soon.

To get swagger.json file on BE, I added [Microsoft.Azure.WebJobs.Extensions.OpenApi](https://github.com/Azure/azure-functions-openapi-extension) NuGet package.

## Fun facts
- I started the implementation using MongoDB driver for CosmosDB. But few hours later I found that it didn't support triggers, so I couldn't implement listener. I had to rewrite my code using CosmosDB SQL API. 
- I used a separate class (service) for DB CRUD operations. Azure Functions support direct access to DB without it. But I like layered solutions :).
- I added API Management service to use Basic Authentication and some other features. I tried to use Azure AD services to authenticate access to Azure Functions and it worked well fine but I think that it's too complex solution for this kind of project, not on the implementation level, but for administration.
- PrimeNG components were selected because I worked with them a few years before, it's a good set of UI components for Angular.
- *GetHomePage* method is an artifact and it's not used but I like this possibility to return HTML file from Azure Function HTTP Trigger.

## ToDo (items order in the list is not related to the priority)
- add unit tests to BE part
- move secrets to Azure KeyVault service (now all the configuration variables we need to put to Configuration section of Azure Functions)
- add AuthGuard for FE, now the authentication implementation on FE is very simple
- add some automation for FE (separate swagger.json getting operation and API generation)
- UI fixes (if it needs)
- get full statistics about the lifecycle of a message, why the latency is so big, where we loose the time