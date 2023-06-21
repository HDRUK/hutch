
![Hutch](https://raw.githubusercontent.com/HDRUK/hutch/main/assets/Hutch%20splash%20bg.svg)

# 📤🐇 Hutch

**Hutch** is an application stack for **Federated Activities**, such as Analytics, Data Discovery or Machine Learning.

- 🔒 Make your data discoverable **safely** and **securely**, without directly sharing it.
- ✅ Hutch is being developed for use in Trusted Research Environments (TRE) or Secure Data Environments (SDE).
  - It will enable researchers to run various workflows, such as those on [WorkflowHub](https://workflowhub.eu/) or custom workflows produced by researchers themselves.

The stack consists of:
- A submission layer that will be able to receive requests from third-party federated analytics providers as well as directly.
- An Agent that receives queries in the form of RO-Crates and uploads the outputs to a store.
- [WfExS](https://github.com/inab/WfExS-backend) (*Not produced by the Hutch team*).
- A results store, where the results can be held for approval for release by the TRE/SDE.

### Tech Stack

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

# ➡️ Getting Started

User and Developer Guidance can be found in the [documentation](https://hdruk.github.io/hutch).

# 📁 Repository contents

| Path | Description | Notes |
|-|-|-|
| `app/HutchManager` | (**Deprecated**) .NET6 Backend API | |
| `app/HutchAgent` | .NET6 Agent | <ul><li>Unpack incoming workflow RO-Crates</li><li>Trigger workflow execution</li><li>Deposit results in the results store</li></ul> |
| `app/manger-frontend` | (**Deprecated**) React (Vite) Client SPA for the Manager | |
| `app/rquest-omop-worker`| Source code for RQuest federated discovery app | Used in example workflows in `workflows` |
| `.github` | GitHub Actions | workflows for building and deploying the applications |
| `assets` | Asset source files | Logos etc. |
| `website` | Docusaurus 2 site | The source for https://hdruk.github.io/hutch |
| `workflows` | Example CWL workflow files | |

# ⚙️ App Configuration

Configuration guidance for the apps in the stack can be found in the documentation:

- [Agent](https://hdruk.github.io/hutch/docs/users/getting-started/configuration/agent)

# ⚖️ License and Contribution

❤️ Hutch is Open Source software under the permissive MIT license.

📜 Hutch © 2022 University of Nottingham.

👷🏾‍♂️ Contributions are currently managed via the development team in the University of Nottingham Digital Research Service.

# Associated Organisations
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
