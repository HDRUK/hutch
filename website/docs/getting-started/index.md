---
sidebar_position: 1
---

# Introduction

Hutch is an open-source tool that enables federated activities on your data. Third parties can run analyses, train machine learning models and much more against your data without it ever leaving your custody.

Hutch is ideal for Trusted Research Environments (TREs) or Secure Data Environments (SDEs).

✅ No need to grant any external systems or people access to your data.<br />
✅ No need to share your data with any other parties.<br />
✅ No need to allow inbound requests to your network.

## The Architecture of Hutch

Hutch is part of an "application stack" that was defined by the TRE-FX Project. Hutch itself provides an implementation of the "Workflow Executor" module of that stack, interacting in a standard with the other modules, and leveraging existing tools to achieve its functionality.

This affords it a great deal of flexibility in terms of how users may want to run it in different setups and enables it to be easily extended to support new formats or functionality.

The main components of the TRE-FX stack are as follows:
- The Submission Layer
- The TRE Controller (Hutch interacts with this)
- The Workflow Executor (Hutch fulfills this module)
- An Intermediary Store (Hutch interacts with this)

Additionally, depending on how (or if!) you implement other areas of the stack, and what your needs are regarding "airgapped" environments, you may require your own:
- Container registry
- Workflow repository

### The Submission Layer

The submission layer is an interoperable part of the stack which can accept requests from various federated activity providers in the form of an RO-Crate. It sits just outside the TRE/SDE, still only allowing outbound requests for activities to be run. External services cannot directly put jobs into the TRE/SDE to be run.

The submission layer aims to reduce or prevent "vendor lock-in" whereby you enable federated activities only with researchers with access to one specific product. This increases the reach of your data in the research community.

### The TRE Controller

The TRE Controller sits inside the TRE/SDE to verify and approve incoming jobs and facilitate the approval of data egress within existing TRE workflows.
It is the only module within the stack that is allowed outside communication (specifically with the Submission Layer) from inside the TRE.

### The Workflow Executor (e.g. Hutch)
The Workflow Executor runs workflow jobs passed to it from the TRE Controller. The RO-Crate in the request is unpacked and, in the case of Hutch, executed using WfExS. Upon completion of the job, outputs are placed in the Intermediary Store and the TRE Controller is notified that they are ready to be inspected for data egress approval. If approved, the results are merged back into the original RO-Crate and returned to the Intermediary Store ready for egress.

### The Intermediary Store

This store serves as a place to put the crates for job requests, outputs of executions, and final results crates. Essentially any transfer between the TRE Controller and the Workflow Executor (Hutch) can be performed via this store.

## Hutch Implementation Specifics

Hutch implements the **Workflow Executor** part of the stack, and interacts with the **TRE Controller** and **Intermediary Store**, as well as optional peripheral components.

Hutch itself is a .NET application, running ASP.NET Core to allow it to provide a REST API for certain interactions.

### Accepting jobs
**Hutch** expects a **TRE Controller** to `POST` jobs to a jobs endpoint over HTTPS.

**Hutch** will verify that incoming jobs meet the **TRE-FX 5 Safes RO-Crate Profile** in the expected state, and if valid will execute the requested workflow.

**Hutch** depends on a local **RabbitMQ** instance for managing its own jobs queue.

### Executing jobs
**Hutch** is capable of executing workflows contained within the crate, fetching workflows from a public source such as **WorkflowHub**, or (as per TRE-FX requirements to support airgapped environments) fetching only approved workflows from an HTTP source within the airgapped environment. Workflows are expected to be in a **Workflow Profile RO-Crate**.

**Hutch** executes **CWL** or **Nextflow** workflows using **WfExS**. **WfExS** is an open source python application that supports CWL and Nextflow workflows, and supports running those workflows in Containers via a number of different container engines such as **docker** and **podman**.

**Hutch** typically uses **WfExS** with **podman** due to its better support for airgapped environments. **Podman** supports fetching Container Images (such as those for tools used by workflows) from local registries inside an airgapped environment.

**Hutch**'s documentation uses **Sonatype Nexus** to fill the role of a local airgapped worflow store and container registry, but these can be fulfilled by other tools as desired.

### Status and Results
**Hutch** interacts with the **TRE Controller**'s REST API to provide status updates.

**Hutch** provides workflow outputs to the **TRE Controller** via REST API / the **Intermediary Store**, to enable disclosure checking to approve outputs for egress.

**Hutch** today can use the **AWS S3 API** (e.g. with a **MinIO** server) as an **Intermediary Store**.

