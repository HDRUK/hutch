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

  "IdentityProvider": {
    "OpenIdBaseUrl": "", // e.g. https://keycloak.tre.com/realms/tre-fx
    
    // If you want Hutch to use OIDC for Minio credentials,
    // this must match the Minio OIDC Client ID!
    // Otherwise it can be a Hutch specific client
    "ClientId": "",

    // May be optional depending on the IdP client config
    // If required and using OIDC for Minio credentials,
    // this must match the Minio OIDC Client Secret!
    "ClientSecret": "",
    
    // User credentials Hutch will act on behalf of
    "Username": "",
    "Password": ""
  },

  // Configuration for tracking Workflow Execution
  // Currently WfExS specific
  "WorkflowExecutor": {
    "ExecutorPath": "/WfExS-backend",
    "VirtualEnvironmentPath": "/WfExS-backend/.pyWEenv/bin/activate",
    "LocalConfigPath": "workflow_examples/local_config.yaml",
    "ContainerEngine": "docker", // other valid options are "singularity" and "podman"

    // The below are more for development / debugging

    // Really we always want a full crate, but some wfexs configs
    // particularly with certain container engines
    // are unreliable with `--full`` on or off, so it can be configured for testing.
    "SkipFullProvenanceCrate": false,
    
    // by default Hutch detaches from the wfexs process once it starts it,
    // to free up threads.
    // This forces Hutch to keep the executing thread attached to the wfexs process
    // which means you can see stdout/stderr from wfexs in realtime,
    // and better understand the circumstances under which wfexs exited.
    // Intended for dev/test use while executing one job at a time.
    "RemainAttached": false,
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
  },

  // Development Flags
  // These are really intended for development or debugging use
  // and their continued presence cannot be relied upon from one build to the next!
  "Flags": {
    "StandaloneMode": false, // Hutch will skip TRE Controller interactions
    "RetainFailures": false, // Hutch will not clean up working directories or database records for jobs that fail.
    "
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
