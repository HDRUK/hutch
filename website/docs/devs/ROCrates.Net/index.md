---
sidebar_position: 4
---

import { SectionUnderConstruction } from "@site/src/components/admonitions/under-construction.tsx";

# ROCrates.Net

<SectionUnderConstruction />

`ROCrates.Net` is a class library written in C# using the .NET Core framework. It aims to provide .NET Core apps with functionality to programmatically build and/or parse [RO-Crates](https://www.researchobject.org/ro-crate/). It is implemented as a C# port of the Python library [`rocrate`](https://pypi.org/project/rocrate/). 

## Add `ROCrates.Net` to your project
To add `ROCrates.Net` to your project from your machine, add the following `ItemGroup` to your project's `.csproj`, substituting `<path/to>` with the actual path:.

```xml
<ItemGroup>
  <ProjectReference Include="<path/to>/ROCrates.Net/ROCrates.Net.csproj" />
</ItemGroup>
```
:::info TBD
### Add `ROCrates.Net` to your project from NuGet

```xml
<ItemGroup>
  <ProjectReference Include="ROCrates.Net" Version="x.y.z"/>
</ItemGroup>
```
:::

:::caution
It is not currently feature complete with `rocrate` yet!
:::

## Creating an RO-Crate
Creating a crate will work similarly to this: https://github.com/ResearchObject/ro-crate-py#creating-an-ro-crate.

## Consuming an RO-Crate
Consuming an crate will work similarly to this: https://github.com/ResearchObject/ro-crate-py#consuming-an-ro-crate.
