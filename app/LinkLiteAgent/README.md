# `linkliteagent`
## About
This is the source code for the Link Lite agent. The agent will:
  - read data requests from a queue,
  - find the data in a database,
  - perform obfuscation, low count filtering, etc,
  - place results in a results queue.

## Development
### Installation
First, make sure you install [`poetry`](https://python-poetry.org/docs/#installation).

Once you have `poetry` installed, run:
```shell
poetry install
```
and this will set up your environment.

### Running `linkliteagent`
To run `linkliteagent`, run:
```shell
poetry run linkliteagent
```
and use `Ctrl+C` to shut it down.

### Testing and Building
To run the tests and build `linkliteagent`'s `sdist` and `wheel`, run:
```shell
poetry run tox
```
That's it!

### Adding and removing deps
To add a dependency, run:
```shell
# For requirements
poetry add <package_name>
# For dev-only
poetry add --dev <package_name>
```
To remove a dependency, run:
```shell
# For requirements
poetry remove <package_name>
# For dev-only
poetry remove --dev <package_name>
```