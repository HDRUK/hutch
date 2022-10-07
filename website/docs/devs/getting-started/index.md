---
sidebar_position: 2
---

# Getting Started

Here's how to run a local development stack, and guidance on developing different specific parts of the stack

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
