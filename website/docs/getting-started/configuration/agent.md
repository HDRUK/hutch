---
sidebar_position: 2
---

# Hutch Agent

The agent can be configured via its `appsettings*.json` files or .NET user secrets.

## Available values

```json
{
  // Local working paths used by Hutch itself
  "Paths": {
    "WorkingDirectoryBase": "hutch-workdir", // Hutch's working directory
    "Jobs": "jobs" // Sub-directory for per-job working directories
  },

  // Configuration for Hutch's internal Action Queue (RabbitMQ)
  "Queue": {
    "Hostname": "", // Rabbit MQ Host
    "Port": 0, // Rabbit MQ Port
    "UserName": "", // Rabbit MQ User
    "Password": "", // Rabbit MQ Password

    "QueueName": "WorkflowJobActions",
    "PollingIntervalSeconds": 5, // How often Hutch checks the internal queue for new Actions
    "MaxParallelism": 10 // How many actions from the queue will Hutch run simultaneously
  },

  // MinIO Intermediary Store Defaults
  // These are are used for Egress in Standalone Mode
  // And as a fallback for Submissions/Egress when only partial bucket details are provided.
  "StoreDefaults": {
    "Host": "localhost:9000",
    "AccessKey": "accesskey",
    "SecretKey": "secretkey",
    "Secure": false,
    "Bucket": "hutch.bucket"
  },

  // Configuration for tracking Workflow Execution
  // Currently WfExS specific
  "WorkflowExecutor": {
    "ExecutorPath": "/WfExS-backend",
    "VirtualEnvironmentPath": "/WfExS-backend/.pyWEenv/bin/activate",
    "LocalConfigPath": "workflow_examples/local_config.yaml",
    "CrateExtractPath": "/WfExS-backend/workflow_examples/ipc/"
  },

  // Connection strings for different services
  "ConnectionStrings": {
    // The database tracking the jobs in the agent
    "AgentDb": "Data Source=HutchAgent.db"
  },

  // Configurable details to add to published Results Crates.
  "Publisher": {
    "Name": "" // Desired Name of the Publisher in Results Crates.
  },
  "License": {
    "Uri": "", // A URI to be used as th License `@id` in Results crate metadata
    "Properties": {} // Any valid CreativeWork properties as desirable to be included for the License.
  }
}
```

## Guidance
### Results Store
- You must choose either a MinIO Results Store or a File System Results Store. You cannot configure both at the same time.

### WfExS
- The `ExecutorPath` must be the directory where WfExS is installed.

- The `VirtualEnvironmentPath` must be the path to the `activate` script in the WfExS install directory, e.g. `/path/to/WfExS-backend/.pyWEenv/bin/activate`.

- The `LocalConfigPath` is the path of a YAML file describing your WfExS installation.

- The `CrateExtractPath` is the path where inbound RO-Crates will be unpacked so that they can be executed by WfExS.

## Watch Folder
- This is where the results of WfExS runs will be saved before being moved to the Results Store.
