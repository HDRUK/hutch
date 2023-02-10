---
sidebar_position: 2
---

# Running WfExS

For a full description of all functions offered by WfExS, refer to the README at [https://github.com/inab/WfExS-backend](https://github.com/inab/WfExS-backend).

To run a workflow using WfExS, first stage a workflow:
```shell
./WfExS_backend.py -L <path_to_wfexs_config.yaml> stage -W <stage_file.yaml>
```
This will prepare the workflow to run and provide a pithy random name to refer to when executing the workflow.

To run the workflow use the following command:
```shell
./WfExS_backend.py -L <path_to_wfexs_config.yaml> staged-workdir offline-exec "pithy random name"
```

And finally, to package the workflow into an RO-Crate, which will include references to the inputs, outputs and workflows, run:
```shell
./WfExS_backend.py \
  -L <path_to_wfexs_config.yaml> \
  staged-workdir create-prov-crate "pithy random name" ROCrate.zip \
  --full
```

## Configuring WfExS

Congiguration in WfExS is done using YAML (`.yaml/.yml`) files. There are two main configuration files to write in order to run WfExS:
* a local configuration file, specified by the `-L` flag after `.WfExS_backend.py` and before any subcommands like `stage`, etc.
* a workflow configuration file or a staging file, passed to `stage` using the `-W` flag.

The local configuration file is used to tell WfExS to find certain commands like `java`, `git`, `singularity` etc. It also specifies the working and cache directories.

The workflow configuartion file is used to specify parameters and oututs of a workflow, as well as other configuration specific to the workflow being run.

More information can be found at [https://github.com/inab/WfExS-backend#configuration-files](https://github.com/inab/WfExS-backend#configuration-files).
