---
sidebar_position: 2
---

# Configuration

:::danger Deprecated
The Hutch Manager is now deprecated and will be removed soon.
:::

The app can be configured in any standard way an ASP.NET Core application can. Typically from the Azure Portal (Environment variables) or an `appsettings.json`.

## Available values and defaults

```yaml
Root:
  Username: "" # Super Admin username. Prefix '@' will be added to the username. If not supplied, 'admin' will be used, which becomes '@admin'.
  Password: "" # Super Admin password
  EmailAddress: "" # If not supplied, 'admin@local' will be used.

ConnectionStrings:
  Default: "" # the main application SQL Server database
Serilog:
  # ...
OutboundEmail:
  ServiceName: Hutch
  FromName: No Reply
  FromAddress: noreply@example.com
  ReplyToAddress: ""
  Provider: local

  # If Provider == "local"
  LocalPath: ~/temp

  # If Provider == "sendgrid"
  SendGridApiKey: ""

  # If Provider == "smtp"
  SmtpHost: "" # SMTP host name
  SmtpPort:    # SMTP port
  SmtpUsername: "" # SMTP username
  SmtpPassword: "" # SMTP password
  SmtpSecureSocketEnum: # for example, assign 2 to implement SslOnConnect
  # Secure socket options
  # 1 - Auto
  # 2 - SslOnConnect
  # 3 - StartTls
  # 4 - StartTlsWhenAvailable

  # More information can be found here
  # http://www.mimekit.net/docs/html/T_MailKit_Security_SecureSocketOptions.htm

UserAccounts:
  RequireConfirmedAccount: # true or false. Check 'AccountConfirmed' status if true
  RequireConfirmedEmail: # true or false. Send confirmation email for user accounts if true.
  Registration: # set as 'free', 'limited' or 'disabled'
  # 'free' - allows new account registration
  # 'limited' - allows registration if email in AllowList
  # 'disabled' - disables registration altogether
  

ActivitySourcePolling:
  PollingInterval: 5 # set to a negative value will disable polling altogether

RquestTaskApi:
  BaseEndpoint: "bcos-rest/api/task"
  QueueStatusEndpoint: "queue"
  FetchQueryEndpoint: "nextjob"
  SubmitResultEndpoint: "result"
  Username: ""
  Password: ""

# Set-up if using RabbitMQ
JobQueue:
  Provider: "RabbitMQ"
  HostName: ""
  Port: 5672
  UserName: "guest"
  Password: "guest"

# Set-up if using Azure Queue Storage
JobQueue:
  Provider: "AzureQueueStorage"
  ConnectionString: "<your connection string>"

# Opt in feature flags
# sometimes features here are works in progress
FeatureManagement:
  # no feature flags currently in use
```

## Sample Production Configuration guidance

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
