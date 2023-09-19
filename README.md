
![Hutch](https://raw.githubusercontent.com/HDRUK/hutch/main/assets/Hutch%20splash%20bg.svg)

# 📤🐇 Hutch

**Hutch** is part of an application stack for **Federated Activities**, such as Analytics, Data Discovery or Machine Learning.

It is an implementation of the **Executing Agent** module of the TRE-FX stack.

- 🔒 Make your data discoverable **safely** and **securely**, without directly sharing it.
- ✅ Hutch is being developed for use in Trusted Research Environments (TRE) or Secure Data Environments (SDE).
  - It will enable researchers to run various workflows, such as those on [WorkflowHub](https://workflowhub.eu/) or custom workflows produced by researchers themselves.

The **TRE-FX Federated stack** consists of:
1. A public **Submission Layer** implementation in which projects and users are configured, managed and authorised. This layer receives job requests as TRE-FX 5 Safes RO-Crates.
2. A **TRE Agent** implementation, which interacts with the Submission Layer to receive jobs and validate them in the context of a specific TRE or other secure environment.
3. An **Executing Agent** implementation that accepts jobs from the TRE Agent, executes them and records outputs and provenance, submitting the results to be approved for egress.
  - [WfExS](https://github.com/inab/WfExS-backend) (*Not produced by the Hutch team*).
4. An **Intermediary Store**, where incoming jobs and outgoing results can be held as they are passed between systems for any purpose e.g.
  - queuing approved jobs to be executed.
  - queueing annotated outputs for disclosure checking and egress approval.
  - queueing an approved results package for egress.

**Hutch** is an implementation of `3.` - The **Executing Agent**.

# ➡️ Getting Started

User and Developer Guidance can be found in the [documentation](https://hdruk.github.io/hutch).

# 📁 Repository contents

| Path | Description | Notes |
|-|-|-|
| `app/HutchAgent` | .NET6 Agent | <ul><li>Unpack incoming workflow RO-Crates</li><li>Trigger workflow execution</li><li>Deposit results in the results store</li></ul> |
| `.github` | GitHub Actions | workflows for building and deploying the applications |
| `assets` | Asset source files | Logos etc. |
| `website` | Docusaurus 2 site | The source for https://hdruk.github.io/hutch |

# ⚙️ App Configuration

Configuration guidance for the Hutch Agent can be found in the [documentation](https://hdruk.github.io/hutch/docs/users/getting-started/configuration/agent).

# ⚖️ License and Contribution

❤️ Hutch is Open Source software under the permissive MIT license.

📜 Hutch © 2022 University of Nottingham.

👷🏾‍♂️ Contributions are currently managed via the development team in the University of Nottingham Digital Research Service.

# Associated Organisations and Programmes
[![HDR UK](https://raw.githubusercontent.com/HDRUK/hutch/main/website/static/img/hdruk_logo.svg)][HDR UK Home] &nbsp;
[![University of Nottingham](https://raw.githubusercontent.com/HDRUK/hutch/main/website/static/img/uon_white_text_web.png)][UoN Home] &nbsp;
[![UKRI](https://raw.githubusercontent.com/HDRUK/hutch/main/website/static/img/UKRI_logo.jpeg)][UKRI Home] &nbsp;
[![DARE UK](https://raw.githubusercontent.com/HDRUK/hutch/main/website/static/img/DARE-UK_logo.png)][DARE UK Home] &nbsp;
[![TRE-FX](https://raw.githubusercontent.com/HDRUK/hutch/main/website/static/img/tre-fx_logo.svg)][TRE-FX Home]

[HDR UK Home]: https://www.hdruk.ac.uk/
[Cohort Discovery]: https://www.healthdatagateway.org/about/cohort-discovery
[UoN Home]: https://nottingham.ac.uk
[UKRI Home]: https://www.ukri.org/
[DARE UK Home]: https://dareuk.org.uk/
[TRE-FX Home]: https://trefx.uk/
