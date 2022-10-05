---
sidebar_position: 2
---

# Hutch Agent

The agent is configured by environment variables; for development it will load a `.env` file local to `pyproject.toml`.

## Available values and defaults

Example .env for development (also documents unnecessary/default values)

```sh
# Logging Database configuration

# LOG_DB_DRIVERNAME="postgresql" # SQLAlchemy driver names (including short names). See currently supported list.
LOG_DB_HOST="localhost"
# LOG_DB_PORT=<driver default> # will use the default port of the database driver
LOG_DB_DATABASE="postgres"
LOG_DB_USERNAME="postgres"
LOG_DB_PASSWORD="example"


# Data source configuration

DATASOURCE_NAME="jobs"
# DATASOURCE_DB_DRIVERNAME="postgresql"
DATASOURCE_DB_HOST=""
# DATASOURCE_DB_PORT=<driver default> # will use the default port of the database driver
DATASOURCE_DB_DATABASE=""
DATASOURCE_DB_USERNAME=""
DATASOURCE_DB_PASSWORD=""
# DATASOURCE_DB_SCHEMA=<driver default> # will use the default schema of the database driver (e.g. `public` for postgres, `dbo` for MSSQL...)


# Manager related configuration

MANAGER_URL="https://localhost:45588"
MANAGER_VERIFY_SSL=0 # Disable SSL verification ONLY IN DEVELOPMENT to allow for self-signed certs. Actual in-app default is 1.

# Check In schedule

# CHECKIN_CRON="0 */1 * * *" # once every hour


# Feature Flags

# USE_RO_CRATES=0 # Use RO CRATES Query and Results Schema internally, instead of Rquest
# USE_RESULTS_MODS=0 # Whether to run results modifiers or not when executing a query
```

### Currently supported SQLAlchemy drivers

We currently only depend on `psycopg2` so that's the only supported driver at this time.

Valid values:

- `postgresql`
- `postgresql+psycopg2`

## Example Production Configuration guidance

Set up the logging DB:
- `LOG_DB_HOST=<host>`
- `LOG_DB_DATABASE=<db name>`
- `LOG_DB_USERNAME=<username>`
- `LOG_DB_PASSWORD=<password>`
  - **Generate a good random one!**
- These settings may be different to the ones in [Database](#database)

Set up the data source configuration:
- `DATASOURCE_NAME=<name>`
  - this will also be the name of the queue on the message queue server for the data source
- `DATASOURCE_DB_HOST=<host>`
- `DATASOURCE_DB_DATABASE=<db>`
- `DATASOURCE_DB_USERNAME=<username>`
- `DATASOURCE_DB_PASSWORD=<password>`
- The `DATASOURCE_DB_` settings may be different to the ones in [Database](#database)

Set up with manager connection:
- `MANAGER_URL=<url to manager>`
  - `https://<host>[:port]` NB: if using docker-compose, `host` is the service name

Set up the message queue connection:
- `MSG_QUEUE_HOST=<host>`
  - the host of the rabbitmq server. If using docker-compose, use the queue service name. Needs to match `ASPNETCORE_JobQueue__HostName` above.
