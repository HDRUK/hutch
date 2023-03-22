---
sidebar_position: 4
---

# Running HutchWorker workflow

## Rquest Omop Worker Workflows
### Main Workflow

### CommandLineTool Workflow

## Configuration
### Set up local DB
- Create a docker image for a postgres DB [docker postgres image](https://hub.docker.com/_/postgres). Point the DB volume to the directory with the sample data csv files.

- Instructions on setting up the sample data once the DB is created can be found [here](../../../../docs/users/sample-data).

### Example stage file used for executing rquest-omop-worker (.yaml)

``` yaml
workflow_id: '<URI to cwl workflow>' # github public url/Workflow RO-Crate file/ Github repo URL
workflow_config:
  container: 'docker' # choice of 'singularity', 'docker', 'podman' or 'none'
  secure: false
nickname: 'hutch-rquest-worker' # prefix for the randomly generated nickname
cacheDir: /tmp/wfexszn6siq2jtmpcache
crypt4gh: 
  key: cosifer_test1_cwl.wfex.stage.key
  passphrase: mpel nite ified g
  pub: cosifer_test1_cwl.wfex.stage.pub
  # taken from an existing example stage file
outputs:
  output_file:
    c-l-a-s-s: File
    glob: "output.json" # needs to match workflow output
params:

  body: '{...}' # contains rquest query
  is_availability: # true or false 
  # Credentials needed to connect to local DB with sample data.
  db_host: ""
  db_name: ""
  db_user: ""
  db_password: ""

```

### WfExS config file
Local installation configuration files can be found [here](https://github.com/inab/WfExS-backend/tree/main/workflow_examples).



