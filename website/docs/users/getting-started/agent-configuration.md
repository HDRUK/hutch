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
