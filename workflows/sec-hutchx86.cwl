cwlVersion: v1.0
class: Workflow
id: hutch_workflow
label: hutch-workflow

inputs:
    body:
        type: File
    result_modifiers:
        type: string?
    is_availability:
        type: boolean
    results:
        type: string?
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
        outputSource: rquest_oneshot/output_file

steps:
    rquest_oneshot:
        run: ./rquest-oneshotx86.cwl
        in:
            body: body
            result_modifiers: result_modifiers
            is_availability: is_availability
            results: results
            db_host: db_host
            db_name: db_name
            db_user: db_user
            db_password: db_password
            
        out: [output_file]
