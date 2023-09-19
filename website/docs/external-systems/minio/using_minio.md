---
sidebar_position: 1
---

# Using MinIO

## Installation
The easiest way to install MinIO is by using Docker.
```shell
docker pull minio/minio
```
Information about the image as well as other versions are available [here](https://hub.docker.com/r/minio/minio/).

## Running MinIO
When running MinIO in Docker, you expose 2 ports and map them to ports `9000` and `9001` inside the container. The following command will let you view the web console at `localhost:9001` in the browser. The object store will be available at `localhost:9000`.
```shell
docker run -p 9000:9000 -p 9001:9001 \
  quay.io/minio/minio server /data --console-address ":9001"
```

If you use `docker-compose`, a MinIO service might look like this:
```yaml
minio:
    image: minio/minio
    restart: always
    ports:
      - "9000:9000" # S3 API
      - "9001:9001" # web console
    volumes:
      - $HOME/minio-data:/data # (Optional) a persistent volume on your host machine
    command: minio server /data --console-address ":9001"
```

## Creating a store
First, log into MinIO at `localhost:9001` (or the port you mapped the web console to on the host machine) with the username `minioadmin` and the password, also `minioadmin`. Change these for increased security.

Navigate to "Buckets" and click "Create Bucket". Give the bucket a name and click "Create Bucket" at the bottom of the form.

---

![](/images/minio-create-bucket.png)

## Creating an Access Key
Navigate to "Access Keys" in the console and click "Create access key". Then click "Create" at the bottom of the form. You will be offered a download of the credentials, which can be used with your MinIO SDK of choice.

---

![](/images/minio-access-key.png)
