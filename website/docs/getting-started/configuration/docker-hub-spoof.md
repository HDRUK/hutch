---
sidebar_position: 4
---

# Docker Images in a TRE

:::info
This page assumes you are using Ubuntu Linux as your OS and that you will have `root` or `sudo` privileges.
:::

Inside a Trusted Research Environment (TRE) there will be no internet access. This means that images cannot be pulled from Docker Hub or any other remote container registry.

**The Docker client cannot be made to point to a custom registry by default**. This means operations like `docker pull postgres` will always try to get `postgres`, for example, from Docker Hub. To get `postgres` from a custom registry, you need to do `docker pull my_custom_registry/postgres`. Many workflows use Docker images from various registries and it would be time-consuming to manually alter them. Furthermore, doing it programmatically would be impractical.

## Use `podman` as an alternative
[`podman`](https://podman.io/) is a free and open-source container runtime. It works like a drop-in replacement for Docker and can pull images from all the same places as Docker. Crucially, it can be configured to allow unqualified image names to your air-gapped container registry.

Instructions to install `podman` can be found [here](https://podman.io/docs/installation)

:::info
To use podman without being `root` or putting `sudo` before each `podman` command, your user must be in the `sudo` group.
:::

### Example `podman` configuration file
The following example shows how to point `podman` to an insecure (non-HTTPS) registry on your local machine. It assumes the registry is running on port `8082`.

In either `/etc/containers/registries.conf` or `$HOME/.config/containers/registries.conf`, add the following:

```toml
unqualified-search-registries = ['localhost:8082']

[[registry]]
location = "localhost:8082"
insecure = true
```

In this example, if you do `podman pull postgres` and `postgres` exists in your registry at `localhost:8082`, the image will be pulled. `podman` will **not** fall back on Docker Hub.
