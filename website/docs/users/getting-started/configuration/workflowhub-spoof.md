---
sidebar_position: 3
---

# (Optional) WorkflowHub in a TRE

:::info
This page assumes you are using Ubuntu Linux as your OS and that you will have `root` or `sudo` privileges.
:::

When running Hutch in a secure environment with no internet access, you will need to redirect links to some external resources to your internal equivalents. This page will explain how to set up a WorkflowHub alternative using Sonatype Nexus as the store.

## Nexus workflow store

Hutch comes with a `docker-compose` service to run an instance Nexus. This service runs its web portal on port `8081`. You can view it by navigating to `your-host:8081` in your web browser, where `your-host` is the IP address or domain name of the machine being used to run Hutch.

### Creating a file store on Nexus

Create a file store on your Nexus instance called `hutchfiles` following [these instructions](/hutch/docs/devs/external-systems/nexus/file-store).

### Upload trusted workflows to Nexus

Navigate to the upload page for `hutchfiles` on your Nexus instance and upload the workflow files. WorkflowHub workflows have numeric IDs (1, 2, 3, ...) which are found in their URLs on WorkflowHub. Let's name the workflows like `id.crate.zip`, e.g. `123.crate.zip` and save them to the `workflows` directory under `hutchfiles`.

---

![](/images/upload-workflows.png)

## WorkflowHub

### Re-routing traffic to `localhost`
To redirect traffic to WorkflowHub to `localhost` / `127.0.0.1`, edit your `/etc/hosts` file by adding the following:

```
# Redirect WorkflowHub traffic
127.0.0.1 workflowhub.eu
```

### Trust issues
To get Hutch to trust calls to the redirected traffic, you need to generate a self-signed SSL certificate and add it to your machine's trust store. Perform the following steps in the home directory of the user running Hutch, e.g. `/home/foo`.

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
When your workflow looks for, say, `https://workflowhub.eu/workflows/488/ro_crate?version=2`, the changes in `/etc/hosts/` will redirect the traffic to your local machine. We will use Nginx to look up the workflow on our Nexus instance and serve it back to us. In the example `nginx.conf` file below, the first `location` block, matches tries to match the WorkflowHub URL and strip out the relevant workflow ID. It then proxies to the Nexus instance, which serves back the file.

This example `nginx.conf` file can be used with the `docker-compose.yml` file in the Hutch repo. Copy the text below into a file called `nginx.conf` into the home directory of the user running Hutch, e.g. `/home/foo/nginx.conf`. It should be next to `key.pem` and `cert.crt` from the previous section. **You do not need to modify this example code. It is designed to work with the `nginx` service in the `docker-compose.yml` file.**

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
