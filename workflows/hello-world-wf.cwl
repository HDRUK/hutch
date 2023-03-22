cwlVersion: v1.0
class: Workflow
id: hello_world
label: hello_world

inputs:
    message:
        type: string

outputs:
    example_out:
        type: File
        outputSource: hello_world/example_out

steps:
    hello_world:
        run: ./hello-world.cwl
        in:
            message: message
            
        out: [example_out]
