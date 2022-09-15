---
sidebar_position: 3
---

# Installation

To run the Hutch application stack, there are four components that need installing.

It is up to you how you install them, and on what sort of infrastructure.

They all communicate via TCP, so you can put any combination of them on the same machine or different machines, provided they are able to talk to the right ports over a network.

## Hutch Manager

The Hutch Manager is a .NET 6.x web application.

It runs [anywhere .NET 6 does][net6-supported].

[net6-supported]: https://github.com/dotnet/core/blob/main/release-notes/6.0/supported-os.md

### Local installation

#### Prerequisites

- [.NET 6 runtime][get-net6] for your chosen Operating System.
  - on Windows choose the "Hosting Bundle" to include the **ASP.NET Core** runtime.

1. Find a [release of the Manager][manager-releases]
  - Releases with timestamps are frequent releases of the current in development version
  - Releases with version numbers are stable versions

[get-net6]: https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime
[manager-releases]: https://github.com/link-lite/hutch/releases?q=manager&expanded=false

### Cloud

The Manager can easily run anywhere .NET 6 web applications can run.

For example, Azure App Service supports running .NET applications in a "Cloud Native" manner, not requiring a dedicated VM.

### Docker

Docker images are available [here](https://github.com/link-lite/hutch/pkgs/container/hutch%2Fhutch-manager) and can be [configured](manager-configuration) by Environment Variables.

### Network connectivity requirements
It will need outbound connectivity on whatever ports any desired Activity Sources are running on; typically 443 (HTTPS)

## Hutch Agent

The Hutch Manager is a .NET 6.x web application.

### Local installation

### Cloud

Currently there is no supported "Cloud Native" approach to hosting the Hutch Agent.

However, it is possible to run it on a VM ion the Cloud, or since a Docker image is available, Cloud container services (such as Azure Container Apps) should also work well.

### Docker

Docker images are available [here](https://github.com/link-lite/hutch/pkgs/container/hutch%2Fhutchagent) and can be configured by Environment Variables.

### Network connectivity requirements
It will need outbound connectivity on whatever ports any desired Activity Sources are running on; typically 443 (HTTPS)

## Configuration Datastore

The Hutch Manager uses a [PostgreSQL] database as a configuration store, so you'll need an available instance of Postgres.

You can install Postgres [however you want][get-postgres], reusing an existing server if appropriate.

An easy way can be to use [Docker][postgres-docker], configured via environment variables:

- `POSTGRES_USER=<username>`
- `POSTGRES_PASSWORD=<password>`
  - **Generate a good random one!**
- `POSTGRES_DB=<db name>`

## Message Queue

Hutch uses [RabbitMQ] for its Message Queue.

You can install RabbitMQ [however you want][install-rabbitmq], reusing an existing server if appropriate.

An easy way can be to use [Docker][rabbitmq-docker], configured via environment variables:

- `RABBITMQ_DEFAULT_USER=<username>`
- `RABBITMQ_DEFAULT_PASS=<password>`
  - **Generate a good random one!**

[PostgreSQL]: https://www.postgresql.org/
[get-postgres]: https://www.postgresql.org/download/
[postgres-docker]: https://hub.docker.com/_/postgres
[RabbitMQ]: https://www.rabbitmq.com/
[install-rabbitmq]: https://www.rabbitmq.com/download.html
[rabbitmq-docker]: https://hub.docker.com/_/rabbitmq
