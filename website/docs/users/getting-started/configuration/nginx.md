# (Optional) Proxying to host

When running Hutch in a secure environment with no internet access, you will need to redirect links to some external resources to your internal equivalents.

## Docker
To redirect traffic to Docker to `localhost` / `127.0.0.1`, edit your `/etc/hosts` file by adding the following:

```
# Redirect WorkflowHub traffic
127.0.0.1 registry-1.docker.io
```

Then in your `nginx.conf` file, add the following to your server block:
```
# send requests to the v1 docker API
location /v1/ {
  proxy_pass https://localhost:8082;
}

# send requests to the v2 docker API
location /v2/ {
  proxy_pass https://localhost:8082;
}
```
:::note
This example assumes that you have your Docker replacement on port 8082 on your machine. Change it as necessary.
:::

### Nginx + Nexus using Docker Compose
Hutch can use Nexus as a place to store Docker images. If you stand up a Nginx instance in Docker Compose along with your nexus instance, replace `localhost` in the above example with the name of your Nexus service, e.g.:

```
# send requests to the v1 docker API
location /v1/ {
  proxy_pass https://my_nexus:8082;
}

# send requests to the v2 docker API
location /v2/ {
  proxy_pass https://my_nexus:8082;
}
```

The Docker Compose services might look like this:

```yaml
nexus:
  image: sonatype/nexus3:3.52.0
  restart: always
  ports:
    - "8081:8081" # web portal port
    - "8082:8082" # port for the docker registry

nginx:
  image: nginx
  restart: always
  ports:
    - "80:80" # HTTP
    - "443:443" # HTTPS
  volumes:
    - /path/to/nginx.conf:/etc/nginx/nginx.conf:ro
```
