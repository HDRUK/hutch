# `rquest-omop-worker`

This workflow can be used to solve Availablity and Distribution queries from Rquest.

Example workflows can be found on the [Hutch monorepo](https://github.com/HDRUK/hutch).

## Workflow inputs
- `body`: The file path containing the body of the request.
- `results_modifiers`: A JSON string containing results modifier parameters.
- `is_availability`: a flag denoating the query is an availability query (**cannot be used with `is_distribution`**).
- `is_distribution`: a flag denoating the query is an distribution query (**cannot be used with `is_availability`**).
- `results`: (*Optional*) an input that specifies the output path for the results.
- `db_host`: the host for the database containing the OMOP data.
- `db_name`: the name of the OMOP database.
- `db_user`: a username with access to the database.
- `db_password`: the password for the database user.

# Executing using WfExS
An example WfExS stage file to run `rquest-omop-worker` might look like the following:

```yaml
# rquest-omop-worker.stage.yaml

workflow_id: https://raw.githubusercontent.com/HDRUK/hutch/main/workflows/sec-hutchx86.cwl
workflow_config:
  container: 'docker'
  secure: false
nickname: 'myNickName'
cacheDir: /tmp/wfexszn6siq2jtmpcache
crypt4gh:
  key: rquest-omop-worker.stage.key
  passphrase: mpel nite ified g
  pub: rquest-omop-worker.stage.pub
outputs:
  output_file:
    c-l-a-s-s: File
    glob: "output.json"
params:
  body:
    c-l-a-s-s: File
    url:
      - https://raw.githubusercontent.com/HDRUK/hutch/main/workflows/inputs/rquest-query.json
  is_availability: true
  db_host: "localhost"
  db_name: "hutch"
  db_user: "postgres"
  db_password: "example"
```
