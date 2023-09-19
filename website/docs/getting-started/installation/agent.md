---
sidebar_position: 2
---

import { SectionUnderConstruction } from "@site/src/components/admonitions/under-construction.tsx";

# Hutch Agent

<SectionUnderConstruction />

The Hutch Agent is a .NET application written in C#.

### Local installation

Currently, there are no downloadable binaries for the Hutch Agent. To install it locally, clone the [Hutch Repository](https://github.com/HDRUK/hutch). Then in the `app/HutchAgent` directory run the following commands:

- Build program in release mode
```bash
dotnet build --configuration Release
```

- Run the program
```bash
dotnet run
```
