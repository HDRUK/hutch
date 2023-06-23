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

Hutch is an "application stack" rather than just one application.

This affords it a great deal of flexibility in terms of how users may want to run it in different setups and enables it to be easily extended to support new formats or functionality.

The main components of the stack are as follows:
- The submission layer
- The agent
- A results store
- [WfExS](https://github.com/inab/WfExS-backend)

Additionally, you may require your own:
- Container registry
- Workflow repository

### The Submission Layer

The submission layer is an interoperable part of the stack which can accept requests from various federated activity providers in the form of an RO-Crate. It sits just outside the TRE/SDE, still only allowing outbound requests for activities to be run. External services cannot directly put jobs into the TRE/SDE to be run. This is the only application the agent will be able to interact with.

The submission layer aims to reduce or prevent "vendor lock-in" whereby you enable federated activities only with researchers with access to one specific product. This increases the reach of your data in the research community.

### The Agent

The agent sits inside the TRE/SDE and runs workflows passed to it from the submission layer. The RO-Crate in the request is unpacked and executed using WfExS. Upon completion of the job, the results are merged back into the original RO-Crate and stored in the results store.

### Results Store

The results store serves as a place to put the results of analyses to check they comply with governance agreements before releasing them to the outside world. This may be a manual check or any automated process you have in place inside your TRE/SDE.
