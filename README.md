# signalr-be
The main goal of this project is an investigation of possibilities of data exchange using sockets and Azure serverless services.
It presents a simple function to listen the CosmosDB table changes and show them on a form

Tne project is base on:
- **Back-end:** Azure functions (.NET C#), SignalR, API Management, CosmosDB, Static Web site on Storage
- **Front-end:** Angular 14, PrimeNG components, SignalR

I can illustrate the architecture in this picture

![Application architecture](https://notificationapptest.z20.web.core.windows.net/notification-arch.png)

The project is available [online](https://notificationapptest.z20.web.core.windows.net/)
- login: testuser
- password: 73M2EStxAo5w

You can add a message using the same browser and check that the messaged will be appeared in the list.

The latency is big (usually ~2 seconds), but it can be related to a weak Azure resources that I use for this test project.
