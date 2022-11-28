---
sidebar_position: 1
---

# Azure
An Azure build of Hutch deploys the Manager as a web app using Azure Web Apps, accompanied by Azure Queue Storage as the message queue, rather than RabbitMQ. An Agent can be ran as an Azure Functions App with a queue trigger.

You can use [Bicep][bicep-intro] to create the resources for Hutch in Azure. We provide Bicep files in the [repository][hutch-repo]. To deploy these, you will need:
- [Azure CLI][azure-cli]
- [Bicep][install-bicep]
- [A resource group][create-resource-group]
- [Key Vault][keyvault] to store secrets like the Queue Storage connection string and the connection string to your database.

Now you can create the parameter files for the Agent and Manager.

## Agent
Create a JSON file called `agent-settings.json`. The keys will be the setting names from [Hutch Agent][/docs/users/getting-started/configuration/agent.md] and you will set the values as appropriate. Set the Azure Queue Storage [connection string](/docs/users/getting-started/installation/message-queue.md) as well.

## Manager
Create a similar JSON file called `manager-settings.json` and populate it with [Manager settings](docs/users/getting-started/configuration/manager.md). Set the Azure Queue Storage [connection string](/docs/users/getting-started/installation/message-queue.md) as well.

## Create resources
You will need the resource ID you created for your deployment.
For prodcution builds use `.azure/bicep/uat.bicep`, for development builds use `.azure/bicep/dev.bicep`.
```bash
az deployment group create \
  --resource-group <resource_id> \
  --template-file <build_file> \
  --parameters webSettings=@agent-settings.json workerSettings=@manager-settings.json
```

[azure-cli]: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli
[install-bicep]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli
[create-resource-group]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resource-groups-portal
[bicep-intro]: https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep
[hutch-repo]: https://github.com/HDRUK/hutch
[keyvault]: https://learn.microsoft.com/en-gb/azure/key-vault/general/overview
