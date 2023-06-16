---
sidebar_position: 3
---

# (Optional) Proxying to host

:::info
This page assumes you are using Ubuntu Linux as your OS.
:::

When running Hutch in a secure environment with no internet access, you will need to redirect links to some external resources to your internal equivalents.

## WorkflowHub

### Re-routing traffic to `localhost`
To redirect traffic to WorkflowHub to `localhost` / `127.0.0.1`, edit your `/etc/hosts` file by adding the following:

```
# Redirect WorkflowHub traffic
127.0.0.1 workflowhub.eu
```

### Trust issues
To get Hutch to trust calls to the redirected traffic, you need to generate a self-signed SSL certificate and add it to your machine's trust store.

1. Generate the certificate.

```bash
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.crt -sha256 -days 365
```
:::info
You will be asked for some information about your organisation etc. Most of this can be left blank except for the "Common Name" which **must** be set to `workflowhub.eu`.

:::
2. Decrypt your private key.

```bash
openssl rsa -in key.pem -out new-key.pem && mv new-key.pem key.pem 
```

3. Add the certificate to trust store.

```bash
cp cert.crt /usr/share/ca-certificates/cert.crt
```
:::info
This command should be executed as `root`.
:::

4. Activate the new certificate.

```bash
dpkg-reconfigure ca-certificates
```
:::info
This command should be executed as `root`.
:::

### Re-routing to the workflow store
Then in your `nginx.conf` file, add the following to your server block:
```
# capture workflow ID and redirect the internal store
location ~ ^\/workflows\/([0-9]+)\/ro_crate$ {
  proxy_pass http://nexus:8081/repository/hutchfiles/workflows/$1.crate.zip;
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
    ssl_certificate /etc/nginx/cert.crt;
    ssl_certificate_key /etc/nginx/key.pem;

    # capture workflow ID and redirect the internal store
    location ~ ^\/workflows\/([0-9]+)\/ro_crate$ {
      proxy_pass http://nexus:8081/repository/hutchfiles/workflows/$1.crate.zip;
    }

    # root location must be defined for the set up to work
    location / {
      proxy_pass http://nexus:8081/service/rest/repository/browse/hutchfiles/;
    }
  }
}
```
