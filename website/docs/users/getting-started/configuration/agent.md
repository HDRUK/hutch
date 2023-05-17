---
sidebar_position: 2
---

# Hutch Agent

The agent can be configured via its `appsettings*.json` files or .NET user secrets.

## Available values

```json
{
  // MinIO Results Store
  "ResultsStore": {
    "Provider": "MinIO",
    "Endpoint": "localhost:9000",
    "AccessKey": "accesskey",
    "SecretKey": "secretkey",
    "Secure": false,
    "BucketName": "hutch.bucket"
  },

  // File System Results Store
  "ResultsStore": {
    "Provider": "FileSystem",
    "Path": "path/to/store/"
  },

  // Configuration for tracking WfExS execution
  "Wfexs": {
    "ExecutorPath": "/WfExS-backend",
    "VirtualEnvironmentPath": "/WfExS-backend/.pyWEenv/bin/activate",
    "LocalConfigPath": "workflow_examples/local_config.yaml",
    "CrateExtractPath": "/WfExS-backend/workflow_examples/ipc/"
  },

  // Watch Folder configuration
  "WatchFolder": {
    "Path": "/Users/daniel/Desktop/TestMinioUpload/"
  }
}
```

## Guidance
### Results Store
- You must choose *one* of either a MinIO Results Store or a File System Results Store. You cannot configure both at the same time.

### WfExS
- The `ExecutorPath` must be the directory where WfExS is installed.

- The `VirtualEnvironmentPath` must be the path to the `activate` script in the WfExS install directory, e.g. `/path/to/WfExS-backend/.pyWEenv/bin/activate`.

- The `LocalConfigPath` is the path a YAML file describing your WfExS installation.

- The `CrateExtractPath` is the path where inbound RO-Crates will be unpacked so that they can be executed by WfExS.

## Watch Folder
- This is where results of WfExS runs will be saved before being moved to the Results Store.
