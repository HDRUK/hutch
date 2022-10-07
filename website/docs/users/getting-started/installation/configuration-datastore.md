---
sidebar_position: 3
---

# Configuration Datastore

The Hutch Manager uses a [PostgreSQL] database as a configuration store, so you'll need an available instance of Postgres.

You can install Postgres [however you want][get-postgres], reusing an existing server if appropriate.

An easy way can be to use [Docker][postgres-docker], configured via environment variables:

- `POSTGRES_USER=<username>`
- `POSTGRES_PASSWORD=<password>`
  - **Generate a good random one!**
- `POSTGRES_DB=<db name>`

[PostgreSQL]: https://www.postgresql.org/
[get-postgres]: https://www.postgresql.org/download/
[postgres-docker]: https://hub.docker.com/_/postgres
