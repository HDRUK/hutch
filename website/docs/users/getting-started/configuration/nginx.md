# (Optional) Proxying to host

When running Hutch in a secure environment with no internet access, you will need to redirect links to some external resources to your internal equivalents.

## WorkflowHub
To redirect traffic to WorkflowHub to `localhost` / `127.0.0.1`, edit your `/etc/hosts` file by adding the following:

```
# Redirect WorkflowHub traffic
127.0.0.1 workflowhub.eu
```

Then in your `nginx.conf` file, add the following to your server block:
```
# rewrite and redirect the URL
location = /workflows {
  rewrite ^/([0-9]+)?version=[0-9]+$ http://nexus:8081/repository/hutchfiles/workflows/$1\.crate\.zip;
}
```

:::note
This example assumes that you have your WorkflowHub replacement on port 8081 on your machine. Change it as necessary. It also assumes that you have a file store called `hutchfiles` with the workflows saved under a directory called `workflows`. The file name pattern will match a number followed by `.crate.zip`, e.g. `123.crate.zip`.
:::

## Example `nginx.conf`
```
# nginx.conf
events {
  worker_connections  1024;
}

http {
  server {
    listen 443 ssl;
    ssl_certificate /etc/nginx/cert.pem;
    ssl_certificate_key /etc/nginx/key.pem;

    # rewrite and redirect the URL
    location /workflows/ {
      rewrite ^/([0-9]+)?version=[0-9]+$ $scheme://nexus:8081/repository/hutchfiles/workflows/$1\.crate\.zip break;
    }
  }
}

```
