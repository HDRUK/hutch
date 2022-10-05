---
sidebar_position: 2
---

# Hutch Agent

The Hutch Manager is a .NET 6.x web application.

### Local installation

### Cloud

Currently there is no supported "Cloud Native" approach to hosting the Hutch Agent.

However, it is possible to run it on a VM ion the Cloud, or since a Docker image is available, Cloud container services (such as Azure Container Apps) should also work well.

### Docker

Docker images are available [here](https://github.com/hdruk/hutch/pkgs/container/hutch%2Fhutchagent) and can be configured by Environment Variables.

## Network connectivity requirements
It will need outbound connectivity on whatever ports any desired Activity Sources are running on; typically 443 (HTTPS)
