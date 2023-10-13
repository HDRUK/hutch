---
sidebar_position: 2
---

# Getting Started

Here's how to run a local development stack, and guidance on developing different specific parts of the stack

## Prerequisites

1. **.NET SDK** `7.x`
1. A **RabbitMQ** instance
1. A **WfExS** environment (if you need to be actually running workflows)

### Partial Running

It is possible to run Hutch in various "partial" setups, where it does not fully interact with external services but still performs all its functionality for an end to end job run (within the scope of Hutch's responsibilities, i.e. from job submission to Hutch, through to uploading a final results package to the Intermediary Store).

These partial setups are detailed [here](partial-running.md)

### Optional for running workflows end to end

- **Podman** - typically Hutch will use WfExS with Podman rather than Docker for airgapped environments
- A Local Container Registry for airgapped use - e.g. **Sonatype Nexus**
- A Local Workflow Store for airgapped use - e.g. **Sonatype Nexus**
  - **Nginx** for proxying workflow URLs to the airgapped store
- **Minio** as an **Intermediary Store**
- Data sources for workflows - e.g. a **PostgreSQL** DB

### For developing the documentation website

1. **Node.js** `16.10+`
1. **Corepack** enabled
  - Just run `corepack enable` in a terminal with node in the PATH

> ℹ️
> 
> The provided `docker-compose.yml` provides suitable development instances of many peripheral services:

- Postgres
- RabbitMQ
- Nexus
- Minio
- Nginx
