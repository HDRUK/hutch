---
sidebar_position: 2
---

# Docker on Nexus

## Making a hosted Docker registry
Navigate to the web portal in the web browaser (`localhost:8081`, or the port you mapped the portal to on the host) and sign in.

:::note
The first time you attempt to login, it will give you the username and tell you to find the admin password (see above). Once logged in you will be promtped to provide a new password for the admin user.
:::

Go to the server administration tab and select "Repositories" under "Repository".

---

![](/images/find-repos.png)

Click "Create repository" and the select "Docker (hosted)" from the list.

---

![](/images/create-repo.png)

![](/images/repo-kind.png)

Give the repository a name and allocate one of the additional ports you specified when running the container. You can either do HTTP (not recommended for production), and/or HTTPS. You will need a separate port for each.

---

![](/images/setup-repo.png)

## Making a Proxy for Docker Hub
To create a Docker Hub proxy, follow the above steps. However, this time choose "docker (proxy)" from the menu. Set up the port as above. Under the "Proxy" settings, add "https://registry-1.docker.io" under "Remote storage". Tick "Use the Nexus Repository truststore" and select "Use Docker Hub".

---

![](/images/docker-proxy-settings.png)

When you pull from the proxy registry, the images are cached on Nexus.

---

![](/images/proxy-contents.png)

## Pushing and pulling the Docker registry
:::note
These steps work for both Hosted and Proxy registries.
:::

### Pushing

First, login to the resgistry.
```shell
docker login localhost:8082
```
Give the user name and password for account on Nexus, such as the admin account, when asked.

Tag your image with the path to the registry.
```shell
docker tag <my image> localhost:8082/<my image>:<some tag>
```

Then push the new tag.
```shell
docker push localhost:8082/<my image>:<some tag>
```

### Pulling
First, login to the resgistry.
```shell
docker login localhost:8082
```

Then pull the new tag.
```shell
docker pull localhost:8082/<my image>:<some tag>
```
