---
sidebar_position: 4
---

# Docker Images in a TRE

Inside a Trusted Research Environment (TRE) there will be no internet access. This means that images cannot be pulled from Docker Hub or any other remote container registry.

**The Docker client cannot be made to point to a custom registry by default**. This means operations like `docker pull postgres` will always try to get `postgres`, for example, from Docker Hub. To get `postgres` from a custom registry, you need to do `docker pull my_custom_registry/postgres`. Many workflows use Docker images from various registries and it would be time-consuming to manually alter them. Furthermore, doing it programmatically would be impractical.

In order to run any images in the TRE without having to alter any image names, **you will need to pre-install them into the local environment before disconnecting it from the internet.** Simply install them locally using `docker pull`. This will save the images locally with names matching what may appear in any workflow.
