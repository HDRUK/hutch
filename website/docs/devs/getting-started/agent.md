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

To add a shared library to the build, such as those in the `lib` directory of the repo, in `pyproject.toml`, go to the section named `[tool.hatch.build.targets.wheel.force-include]` and `[tool.hatch.build.targets.sdist.force-include]` and add the relative path to the packages you wish to include like so: `../../lib/name_of_shared_lib = shared_lib`.

:::info
It is recommended that you add your shared libs to the `lib` folder at the root of the repo. Make sure they contain a `__init__.py` file (which can be empty).
:::

### Adding and removing deps
To add a dependency to the whole project, in `pyproject.toml`, go to `dependencies` in `[project]` and add the name of the package you wish to add to the list. You should also add a version number to get more repeatable builds.

If you want to only add dependencies to a non-default environment, such as `mysql` or `sqlserver`, go to the `[tool.hatch.envs.<env_name>]` sections in `pyproject.toml` and add the dependency to the `extra-dependencies` list. The other dependencies are inherited from the default environment.

Further information about adding dependencies to a hatch `pyproject.toml` can be found [on the hatch site](https://hatch.pypa.io/latest/config/dependency/)
