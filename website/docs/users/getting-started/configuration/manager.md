---
sidebar_position: 1
---

# Hutch Manager

Set the environment to `Production`:
- `ASPNETCORE_ENVIRONMENT=Production`

Set the SSL certificates:
- `ASPNETCORE_Kestrel__Certificates__Default__Path=<path>`
  - **path**: the path to the `.pem` file
- `ASPNETCORE_Kestrel__Certificates__Default__KeyPath=<path>`
  - **path**: the path to the `.key` file
- If you are using Docker, these paths must be the paths to the certs **inside the container**. You will also need to mount the location of the certificate to the container. e.g. the directory for the certificate could be `/certs` inside the container.

Set the database connection string as an environment variable:
- `ASPNETCORE_ConnectionStrings__Default=Host=<host>;Username=<username>;Port=<port>;Password=<password>;Database=<db>`
  - **Host**: the URL to your database server. If running in docker-compose, use the DB service name.
  - **Username**: the username for the DB
  - **Password**: the password for the DB
  - **Port**: the port number for the DB, e.g. 5432 for postgres
  - **Database**: the name of the DB on the server

Set the message queue credentials:
- `ASPNETCORE_JobQueue__HostName=<host name>`
  - the host of the rabbitmq server. If using docker-compose, use the queue service name.
- `ASPNETCORE_JobQueue__UserName=<username>`
  - the username of the message queue. NB: use the same one from [Message Queue](#message-queue)
- `ASPNETCORE_JobQueue__Password=<password>`
  - the password of the message queue. NB: use the same one from [Message Queue](#message-queue)
