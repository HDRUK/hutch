# Hutch Agent

### Installation
First, make sure you install [`hatch`](https://hatch.pypa.io/latest/install/).

Run the following commands in `app/HutchAgent`.

Once you have `hatch` installed, run:
```shell
hatch env create
```
and this will set up your environment to use the agent with RabbitMQ and Postgres.

If you would like to set the agent up to work with another database, run:
```shell
hatch env create mysql
```
for MySQL, or:
```shell
hatch env create sqlserver
```
to work with SQL Server.

### Running `hutchagent`
Run the following commands in `app/HutchAgent`.

To run `hutchagent` with Postgres, run:
```shell
hatch run hutchagent
```
and use `Ctrl+C` to shut it down.

To run `hutchagent` with MySQL, run:
```shell
hatch run mysql:hutchagent
```
Or:
```shell
hatch run sqlserver:hutchagent
```
for SQL Server.

Again, use `Ctrl+C` to shut it down.

### DB for logging
The agent writes logs to a database. It also writes to the standard output as a backup in case the connection fails. The creation of the logging database is handled by `HutchManager` normally, but if you want to work with the agent without the manager for development purposes, perform the following steps:

At the root of the repo run:
```shell
docker-compose up -d
```
and you will get a fresh postgresql db.

Then in `app/HutchAgent`, run:
```shell
hatch run [env:]build-log-db -u <username> -p <password> -d <database> --drivername <drivername>
```
Help can be found by running:
```shell
hatch run [env:]build-log-db --help
```

### Building
```shell
hatch build
```
That's it!

### Adding and removing deps
To add a dependency to the whole project, in `pyproject.toml`, go to `dependencies` in `[project]` and add the name of the package you wish to add to the list. You should also add a version number to get more repeatable builds.

If you want to only add dependencies to a non-default environment, such as `mysql` or `sqlserver`, go to the `[tool.hatch.envs.<env_name>]` sections in `pyproject.toml` and add the dependency to the `extra-dependencies` list. The other dependencies are inherited from the default environment.

Further information about adding dependencies to a hatch `pyproject.toml` can be found [on the hatch site](https://hatch.pypa.io/latest/config/dependency/)
