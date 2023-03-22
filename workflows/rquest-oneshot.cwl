cwlVersion: v1.0
class: CommandLineTool
id: rquest-oneshot
label: rquest-oneshot

hints:
    DockerRequirement:
        dockerPull: pszdldocker/rquest-oneshot:latest

requirements:
    EnvVarRequirement:
        envDef:
            DATASOURCE_DB_HOST: $(inputs.db_host)
            DATASOURCE_DB_DATABASE: $(inputs.db_name)
            DATASOURCE_DB_USERNAME: $(inputs.db_user)
            DATASOURCE_DB_PASSWORD: $(inputs.db_password)

baseCommand: [rquest-omop-agent]
inputs:
    body:
        type: string
        inputBinding:
            position: 1
            prefix: --body
    result_modifiers:
        type: string?
        inputBinding:
            position: 2
            prefix: -m
    is_availability:
        type: boolean
        inputBinding:
            position: 3
            prefix: -a
    results:
        type: string?
        inputBinding:
            position: 4
            prefix: -o
    db_host:
      type: string
    db_name:
      type: string
    db_user:
      type: string
    db_password:
      type: string

outputs:
    output_file:
        type: File
        outputBinding:
            glob: "output.json"
