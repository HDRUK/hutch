---
sidebar_position: 2
---

# Installation

:::danger Deprecated
The Hutch Manager is now deprecated and will be removed soon.
:::

## Prerequisites

1. **.NET SDK** `6.x`
  - The backend API is .NET6 (LTS)
1. **Node.js** `>=16.9`
  - `16.9` and newer include **Corepack**
  - `16.x` is LTS at time of writing
1. **Enable [Corepack](https://nodejs.org/api/corepack.html)**
  - Simply run `corepack enable` in your cli
1. PostgreSQL DB
1. RabbitMQ instance

> ℹ️
> 
> The provided `docker-compose.yml` provides suitable Postgres and RabbitMQ development instances.

## Database setup

The application stack interacts with a PostgreSQL database, and uses code-first migrations for managing the database schema.

When setting up a new environment, or running a newer version of the codebase if there have been schema changes, you need to run migrations against your database server.

The easiest way is using the dotnet cli:

1. If you haven't already, install the local Entity Framework tooling

- Anywhere in the repo: `dotnet tool restore`

1. Navigate to the same directory as `HutchManager.csproj`
1. Run migrations:

- `dotnet ef database update`
- The above runs against the default local server, using the connection string in `appsettings.Development.json`
- You can specify a connection string with the `--connection "<connection string>"` option

## Working with JavaScript

This monorepo uses [pnpm](https://pnpm.io) workspaces to manage JS dependencies and scripts.

Basically, where you might normally use `npm` or `yarn`, please use `pnpm` commands instead.

You don't need to install anything special; Corepack will.

A brief pnpm cheatsheet is provided [here](pnpm-cheatsheet).

## Local installation

Find a [release of the Manager][manager-releases]
- Releases with timestamps are frequent releases of the current in development version
- Releases with version numbers are stable versions

[get-net6]: https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime
[manager-releases]: https://github.com/hdruk/hutch/releases?q=manager&expanded=false

## Cloud

The Manager can easily run anywhere .NET 6 web applications can run.

For example, Azure App Service supports running .NET applications in a "Cloud Native" manner, not requiring a dedicated VM.

:::info Coming soon!
A Cloud native strategy is to use Hutch Manager with a Cloud message queue, such as [Azure Queue Storage](https://azure.microsoft.com/en-gb/products/storage/queues/) or [AWS SQS](https://aws.amazon.com/sqs/), etc.
:::

## Docker

Docker images are available [here](https://github.com/hdruk/hutch/pkgs/container/hutch%2Fmanager) and can be [configured](../configuration/manager) by Environment Variables.

## Network connectivity requirements

It will need outbound connectivity on whatever ports any desired Activity Sources are running on; typically 443 (HTTPS)
