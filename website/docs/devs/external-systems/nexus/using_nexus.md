---
sidebar_position: 1
---

# Using Nexus

## Installation
The easiest way to install nexus is by using Docker.
```shell
docker pull sonatype/nexus3
```
Information about the image as well as other versions are available [here](https://hub.docker.com/r/sonatype/nexus3/).

:::warning
Nexus only has `amd64`-based images, so performance may vary depending on your machine's processor.
:::

## Running Nexus
When running Nexus in Docker, you expose a port and map it to port `8081` inside the container. This will let you view the web console at `localhost:8081` in the browser.
```shell
# map port 8081 on host to 8081 on the container
docker run -p "8081:8081" sonatype/nexus3

# ## OR ##

# map port 1234 on host to 8081 on the container
docker run -p "1234:8081" sonatype/nexus3
```

You will also need to expose additional ports for your repository services as well. Suppose you want to add a docker registry to your Nexus, you could map port `8082`. Additionally you could map `8083` for a git repo on your Nexus.
```shell
docker run -p "8081:8081" -p "8082:8082" -p "8083:8083" sonatype/nexus3
```

If you use `docker-compose`, a Nexus service might look like this:
```yaml
nexus:
  image: sonatype/nexus3
  restart: always
  ports:
    - "8081:8081" # web portal port
    - "8082:8082" # port for the docker registry
    - "8083:8083" # port for the git system
```

## Getting the admin password
The admin password can be obtained by running the following command in the terminal.
```shell
docker exec nexus cat /nexus-data/admin.password 
```
