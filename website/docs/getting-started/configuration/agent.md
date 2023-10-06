---
sidebar_position: 2
---

# Hutch Agent

Hutch can be configured using the following source in [the usual .NET way](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration), in order of precedence:
-  `appsettings.json` adjacent to the binary (`HutchAgent.dll`)
- Environment Variables (with double underscore `__` as a hierarchical separator)
- Command line arguments
- (.NET User Secrets in development)

## Available values

```json
{
  // Kestrel options e.g. port bindings
  // By default Hutch binds on all interfaces on specific non-privileged ports
  // You can change the binding configuration
  // but Hutch should not be bound on privileged ports (< 1024) if you don't want to run it evelated
  // and Hutch should not be bound on 80/443 in airgapped environments where nginx is used to proxy workflow fetching (as nginx will use those ports)
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5209"
      },
      "Https": {
        "Url": "https://0.0.0.0:7239"
      }
    }
  },

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
  },

  // Connection strings for different services
  "ConnectionStrings": {
    // The database tracking the jobs in the agent
    "AgentDb": "Data Source=HutchAgent.db"
  },

  // Configurable details to add to published Results Crates.
  "CratePublishing": {
    "Publisher": {
      "Id": "" // Desired Identifier (typically URL) for the Publisher in Results Crates.
    },
    "License": {
      "Uri": "", // A URI to be used as th License `@id` in Results crate metadata
      "Properties": {} // Any valid CreativeWork properties as desirable to be included for the License.
    }
  }
}
```

## Guidance
### Intermediary Store
- Primarily for Standalone mode or as a fallback; you may configure MinIO connection details to a default store here.

### Workflow Executor
- The `ExecutorPath` must be the directory where WfExS is installed.

- The `VirtualEnvironmentPath` must be the path to the `activate` script in the WfExS install directory, e.g. `/path/to/WfExS-backend/.pyWEenv/bin/activate`.

- The `LocalConfigPath` is the path of a YAML file describing your WfExS installation.
