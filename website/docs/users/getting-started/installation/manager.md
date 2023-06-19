---
sidebar_position: 1
---

import { SectionUnderConstruction } from "@site/src/components/admonitions/under-construction.tsx";

# Hutch Manager

<SectionUnderConstruction />

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
[manager-releases]: https://github.com/hdruk/hutch/releases?q=manager&expanded=false

### Cloud

The Manager can easily run anywhere .NET 6 web applications can run.

For example, Azure App Service supports running .NET applications in a "Cloud Native" manner, not requiring a dedicated VM.

:::info Coming soon!
A Cloud native strategy is to use Hutch Manager with a Cloud message queue, such as [Azure Queue Storage](https://azure.microsoft.com/en-gb/products/storage/queues/) or [AWS SQS](https://aws.amazon.com/sqs/), etc.
:::

### Docker

Docker images are available [here](https://github.com/hdruk/hutch/pkgs/container/hutch%2Fmanager) and can be [configured](../configuration/manager) by Environment Variables.

## Network connectivity requirements

It will need outbound connectivity on whatever ports any desired Activity Sources are running on; typically 443 (HTTPS)

## Migrations

    // TODO
