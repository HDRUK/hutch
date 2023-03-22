---
sidebar_position: 4
---

# Running HutchWorker workflow

## Rquest Omop Worker Workflows
The rquest-omop-worker workflows can be found [here](https://github.com/HDRUK/hutch/tree/main/workflows).
  - `sec-hutch.cwl`: Main workflow linked in `workflow_id` in WfExS stage file.
  - `sec-hutchx86.cwl`: Same as above but configured for intel chip

  - `rquest-oneshot.cwl`: CommandLineTool referenced in main workflow.
  - `rquest-oneshotx86.cwl`: Same as above but configured for intel chip

**Note:** WfExS needs the workflows to be nested as of now, with main workflow linking to the CommandLineTool.

## Configuration
### Set up local DB
- Create a docker image for a postgres DB - [docker postgres image](https://hub.docker.com/_/postgres). Point the DB volume to the directory with the sample data csv files.

- Instructions on setting up the sample data once the DB is created can be found [here](../../../../docs/users/sample-data).

### Stage file for executing rquest-omop-worker

``` yaml
workflow_id: # URL to workflow 
#choice of github public url, workflow RO-Crate zip archive, github repo URL
workflow_config:
  container: # choice of 'singularity', 'docker', 'podman' or 'none'
  secure: false
nickname: 'hutch-rquest-worker' # prefix for the randomly generated nickname
cacheDir: /path/to/chacheDir
crypt4gh: # four random words here
  key: /path/to/private-key
  passphrase: 
  pub: /path/to/public-key
outputs:
  output_file:
    c-l-a-s-s: File
    glob: "output.json" # needs to match workflow output
params: # parameters needed to run the workflow
  body: '{...}' # contains rquest query json
  is_availability: # true or false 
  # Credentials needed to connect to local DB with sample data.
  db_host:
  db_name:
  db_user:
  db_password:
```

### WfExS config file
An example of a local configuration files can be found [here](https://github.com/inab/WfExS-backend/tree/main/workflow_examples). More specificaly `local_config.yaml` is used to stage the rquest-omop-worker workflow also found [here](config.md#local-wfexs-configuration).

## Executing the workflow
Once the DB is set up with the sample data you may execute the workflow using these [steps](running-wfexs.md#running-wfexs). 
- Where `<path_to_wfexs_config.yaml>` use the [local config](#wfexs-config-file).
- Where `<stage_file.yaml>` use the [wfExS stage file](#stage-file-for-executing-rquest-omop-worker).



