---
sidebar_position: 1
---

# Introduction

Hutch is an open source tool that allows you to make your data discoverable without compromising its integrity or security.

✅ You do not need to grant any external systems or people access to your data.<br />
✅ You do not need to share your data with any other parties.<br />
✅ You do not need to allow external services to make incoming requests to your network.

## The Architecture of Hutch

Hutch is an "application stack" rather than just one application.

This affords it a great deal of flexibility in terms of how users may want to run it in different setups, and enables it to be easily extended to support new formats or functionality.

Components of the stack are as follows:

- Manager
- Agent
- Configuration Datastore
- Message Queue
- (Activity Source)
- (Data Source)

The stack features a central **Manager** which is responsible for configuration (via a GUI) and interacting with external **Activity Sources**, queueing jobs when they are picked up.

Jobs are in turn picked up by **Agents** who execute the job in question against a **Data Source** and return the results of execution to the **Manager**, which in turn returns the results to the **Activity Source** the job came from.

### The Manager

The Manager is the only application in the stack that needs to communicate outside of your network. It makes outbound requests only to supported **Activity Sources** to poll for jobs.

The Manager application also serves a web-based GUI, so that you can configure it from the local machine or across a network easily, without the need for a special client installation.

The Manager performs translation between supported Activity Source native formats and Hutch's internal transport formats, facilitating easy interoperability with different Activity Sources.

The Manager requires access to a datastore to save configuration details. This store can be independent of any target Data Sources. The Manager never needs access to your data.

### The Agents

Agents are configured to access a Data Source, and require access to the **Manager** and the Message Queue.

An Agent tells the **Manager** the name of its configured Data Source, and then checks the Queue for any jobs for that **Data Source**. It is possible to run multiple agents for the same **Data Source** concurrently; the queue ensures an individual job will not be picked up by more than one Agent.

The Agent executes the job against the Data Source and returns the results to the Manager. All of this is done using internal transport data formats.

Agents are capable of modifying results prior to returning to the Manager, e.g. an aggregate record count results may be obfuscated to suppress a low number of results below a configured threshold.

### Activity Sources

Activity Sources are external to the Hutch stack (and usually your network). They are the source of job requests to be fulfilled by Hutch.

For example, an **Activity Source** may provide requests to query counts of records within your dataset. Hutch will fulfill that request against your **Data Source** and return the results as configured.

### Data Sources

Data Sources are external to the Hutch stack (but typically within your network). A Data Source contains a dataset belonging to you that you wish to make discoverable.

Only Hutch **Agents** require access to your Data Source for Hutch to work.
