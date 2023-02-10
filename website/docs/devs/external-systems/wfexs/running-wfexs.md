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
