---
sidebar_position: 4
---

# TRE-FX Deployment Notes

This section contains notes specific to the deployment of testing/showcase environments for the TRE-FX project.

They cover running a TRE-FX TRE Agent and the Hutch Agent in separate VMs, configured to talk to a centrally hosted instance of the TRE-FX Submission Layer.

The notes here assume the use of Microsoft Azure Cloud initially.

## VMs

All VMs are Ubuntu Server 22.04 LTS on x64.

In Azure they have all been provisioned as `Standard D4s v3 (4 vcpus, 16 GiB memory)`.

The first VM was set up to a common point as per the below. Then imaged so it could be cloned and set up separately for Hutch and the TRE Agent specifically.

Once a VM pair for both the TRE Agent and Hutch were complete, they were imaged to allow provision of further environments.

### Common Setup

Step by step as follows:

1. ssh with the Azure generated private key
1. `sudo apt update`
1. `sudo apt upgrade -y`
  1. default selection for outdated daemon service restarts

Initial environment state at this point:

```
$ python3 --version
Python 3.10.12
```

```
$ git --version
git version 2.34.1
```

Other Hutch / WfExS dependencies not installed (podman, dotnet, etc...)

### SAIL TRE-Agent VM setup

`// TODO`

The SAIL TRE-Agent stack should all be described in Docker Compose, so we likely only need Docker here, and some network config.

### BitFount Pod TRE-Agent VM setup

Alternatively, the BitFount Pod can be used in the TRE-Agent role.

`// TODO`

It requires Python3 (The `3.10` in the environment should suffice?) and probably nothing else?

Network config TBC.

### Hutch VM setup

Hutch wants the following components, not necessarily in the same VM, but to be accessible:

Same VM:

1. Hutch itself (depends on dotnet7 with the aspnet core runtime)
1. WfExS (depends on Python3)
1. Podman

There is an Ansible playbook that should work for preparing all of this in the environment.

Accessible services:
1. RabbitMQ
1. Sonatype Nexus (for storing container images of approved tools, and Workflow RO-Crates of approved workflows)
1. Minio (the instance(s) that the TRE-Agent expects to use for transport between itself and Hutch - for sending job crates to Hutch, inspecting putative outputs from Hutch (i.e. Disclosure Control), and receiving approved outputs from Hutch)
1. Target datastores - in the context of TRE-FX this means a PostgreSQL Database which credentials will be provided for on a per job basis by the TRE-Agent.

All of the above can be run in the same VM with Docker Compose. You can tell compose to stand up specific services if you don't need the whole stack.

`// Q: Should Minio be in the TRE-Agent VM for this setup?`
