---
sidebar_position: 2
---

# Configuration

## Local WfExS Configuration
The local configuration for WfExS is provided in a YAML (`.yaml`/`.yml`) file. An initial local configuration can be made by running the following command in an empty directory:
```shell
./WfExS_backend.py init
```
An example local config may look like:
```yaml
workDir: ./wfexs-backend-test_WorkDir
cacheDir: ./wfexs-backend-test
crypt4gh:
  key: local_config.yaml.key
  passphrase: strive backyard dividing gumball
  pub: local_config.yaml.pub
tools:
  dockerCommand: docker
  containerType: singularity
  encrypted_fs:
    type: encfs
    command: encfs
  engineMode: local
  gitCommand: git
  javaCommand: java
  singularityCommand: singularity
  staticBashCommand: bash-linux-x86_64
```
Under `tools`, specify the commands to run programs like `singularity`, `java`, `docker`, etc. If only the names are specified, as above, these programs must be in `$PATH`. If any are not in `$PATH`, either add them to `$PATH` or give the full path to the progam here.

## Workflow Configuration
The workflow configuration is also given in a YAML file. Template:
```yaml
workflow_id: # URL to workflow
workflow_config:
  secure: false
cacheDir: /path/to/chacheDir
crypt4gh:
  key: /path/to/private-key
  passphrase: four random words here
  pub: /path/to/public-key
params:
  ...
outputs:
  ...
```
The `params` are the inputs to your workflow. The `outputs` map to the expected files/directories that come out at the end of the workflow.

More information can be found at [https://github.com/inab/WfExS-backend#configuration-files](https://github.com/inab/WfExS-backend#configuration-files).
