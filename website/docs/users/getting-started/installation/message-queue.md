---
sidebar_position: 4
---

# Message Queue

Hutch uses [RabbitMQ] for its Message Queue.

You can install RabbitMQ [however you want][install-rabbitmq], reusing an existing server if appropriate.

An easy way can be to use [Docker][rabbitmq-docker], configured via environment variables:

- `RABBITMQ_DEFAULT_USER=<username>`
- `RABBITMQ_DEFAULT_PASS=<password>`
  - **Generate a good random one!**

# Azure Queue Storage

Alternatively, you can use [Azure Queue Storage][azure-queue-storage] as your message queue. Add the connection string to your [Key Vault][keyvault]:
- `AZURE_STORAGE_CONNECTION_STRING=<connection_string>

[RabbitMQ]: https://www.rabbitmq.com/
[install-rabbitmq]: https://www.rabbitmq.com/download.html
[rabbitmq-docker]: https://hub.docker.com/_/rabbitmq
[azure-queue-storage]: https://learn.microsoft.com/en-au/azure/storage/queues/storage-dotnet-how-to-use-queues?tabs=dotnet
[keyvault]: https://learn.microsoft.com/en-gb/azure/key-vault/general/overview
