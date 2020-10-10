# Server side architecture

The server-side API has several mechanisms.
One is the Register API, which issues a UUID and Secret for API access of AES256 when the application starts to be used.
This is issued as an access key for accessing other APIs.
You can get Microsoft Azure from: All can be started for free and the design is designed to stay within the free usage range.

https://azure.microsoft.com/

## Development architecture
For development, it is designed so that you can pay a small amount.
Both are likely to fit within the free tier.

![Folder](images/DevArchitecture.png)

First, build in the local environment.
[HOW_TO_BUILD_SERVER_SIDE](HOW_TO_BUILD_SERVER_SIDE.md)

### Functions (REST API)
Azure Functions is used as a REST API.
In addition, the batch operation of schedule execution is also performed.
These are not fixed instances, but have a mechanism to automatically scale out according to traffic.
https://docs.microsoft.com/en-us/azure/azure-functions/

### Cosmos DB
The data store for this project.
During development, it is unlikely that the default 400RU will be used up.
https://docs.microsoft.com/en-us/azure/cosmos-db/introduction

### Functions (Batch Job)
Operates as a batch file process.
Generates positive diagnostic key in protocol buffer format.
These persist in storage blobs.
There is also a batch that deletes junk files that are more than two weeks old.
https://docs.microsoft.com/en-us/azure/azure-functions/

### Storage Blob
Blobs are regularly fetched from the client.
https://docs.microsoft.com/en-us/azure/storage/blobs/

## Production architecture
The design example in production is shown below.

![Folder](images/ProdArchitecture.png)

### FrontDoor
Azure Front Door has the functions of traffic distribution, web firewall feature.
Both are evenly distributed to provide automatic switching processing when a region failure occurs.
https://docs.microsoft.com/en-us/azure/frontdoor/

### Functions (REST API)
Azure Functions will only match the special request provided by Front Door and only accept the correct request.
https://docs.microsoft.com/en-us/azure/azure-functions/

### Application Insights
All instances are dynamically increasing and decreasing, so we are monitoring them with Application Insights.
https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview

### Cosmos DB
It syncs to two regions. The endpoint given at this time is an automatic failover endpoint.
Data written between two regions will be synchronized to both sides regardless of which one is written.
https://docs.microsoft.com/en-us/azure/cosmos-db/introduction

### Blob Storage
Blob storage operates in GRS replication mode.
The data written on one side (Japan East in this example) is synchronized with western Japan.
Failover can be performed if necessary.
Since it is actually cached in the CDN, you can only switch the destination of the CDN.
https://docs.microsoft.com/en-us/azure/storage/blobs/

### CDN
It is responsible for caching the generated files in Blob storage.
https://docs.microsoft.com/en-us/azure/cdn/

### Key Vault (HSM)
This application handles a wide variety of keys.
For example, there are Google SafetyNet keys, Apple Device Check keys, and other keys for signing diagnostic keys and keys for verifying each signature.
These are all stored in the Key Vault and can be loaded only by the app that is allowed when the application starts.
https://docs.microsoft.com/en-us/azure/key-vault/

## CI/CD Pipeline

Deployment to these is automated by the CI pipeline.


![Folder](images/CIPipeline.png)

### Azure DevOps - GitHub

Once registered to Master on GitHub, it will also sync to Repos on DevOps.
The opposite is also true.
This is a remnant of Scrum development with the core team for a while.

### Azure DevOps
On the DevOps side, when an update occurs in the server side project under the src folder of the branch master, it is automatically placed in the Dev environment.
https://azure.microsoft.com/en-us/services/devops/

### App Center
App Center is responsible for the client build pipeline.
When the master is updated, the binaries for iOS and Android are automatically built and the startup test etc. are performed.
https://azure.microsoft.com/en-us/services/app-center/

### Prod Pipeline

Once the release branch is updated, it will be pending confirmation by the approvers.
If this approval is passed, it will be deployed to the production environment.
In order to deploy without interruption, first deploy to the Swap slot, and then replace the running instances one by one.

In this pipeline build, the pre-built ZIP is deployed as Run from Package, and it is designed to be able to manually restore it in the event of a malfunction or tampering with the server.

Also, the Swap mechanism allows you to revert to the original state if there is a problem after the Swap.

https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package