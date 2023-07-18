# Exported Crates

Wfexs will let you export ro-crates of different types (with different contents) at a few points along the way.

Here is a brief coverage of the different crates you can get.

| Command | Crate Types | Notes |
|-|-|-|
| `export-stage` | Staged | Must be run on a staged working directory identifier (i.e. after `stage` has occurred) |
| `export-stage --full` | Staged (Full) | As above |
| `export-crate` | Execute | Must be run on an executed working directory (i.e. after `offline-exec` has occurred) |
| `export-crate --full` | Execute (Full) | As above |
| `execute` | Staged, Execute | This also runs `stage` and `offline-exec` for you |
| `execute --full` | Staged (Full), Execute (Full) | This also runs `stage` and `offline-exec` for you |
| `create-prov-crate` | Provenance | Must be run on an executed working directory (i.e. after `offline-exec` has occurred) |
| `create-prov-crate --full` | Provenance (Full) | As above |

| Crate | Description | Commands |
|-|-|-|
| Staged | the source workflows, packed entry worfklow, crate metadata | `stage` + `export-stage`, `execute` |
| Staged (Full) | As above but with inputs and container images | as above but with `--full` |
| Execute | source + packed worflows, crate metadata | `offline-exec` + `export-crate`, `execute` |
| Execute (Full) | As above but with inputs, outputs and container images | as above but with `--full` |
| Provenance | ATM this seems to be the same as Execute. It probably should have different metadata, but doesn't currently. | `offline-exec` / `execute` + `create-prov-crate` |
| Provenance (Full) | ATM this seems to be the same as Execute (Full). It probably should have different metadata, but doesn't currently. | as above with `--full` |
