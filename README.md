
![Hutch](https://raw.githubusercontent.com/HDRUK/hutch/main/assets/Hutch%20splash%20bg.svg)

# 📤🐇 Hutch

**Hutch** is an application stack for **Federated Data Discovery**.

- 🔒 Make your data discoverable **safely** and **securely**, without directly sharing it.
- ✅ Hutch is being developed for use in HDR UK's [Cohort Discovery] toolset.
  - Hutch allows federated access to summary statistics of medical data at institutions.

The stack consists of:
- a Web application "Manager" GUI
  - React frontend
  - .NET backend API
- Agents which retrieve data
  - e.g. a Python app to execute queries against OMOP data in PostgreSQL.
- a Message Queue for queuing agent jobs
  - RabbitMQ

### Tech Stack

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB)
![Python](https://img.shields.io/badge/Python-FFD43B?style=for-the-badge&logo=python&logoColor=blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/rabbitmq-%23FF6600.svg?&style=for-the-badge&logo=rabbitmq&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

# ➡️ Getting Started

User and Developer Guidance can be found in the [documentation](https://hdruk.github.io/hutch).

# 📁 Repository contents

| Path | Description | Notes |
|-|-|-|
| `app/HutchManager` | .NET6 Backend API | |
| `app/HutchAgent` | Python Agent | The agent will:<br /><ul><li>read data requests from a queue</li><li>find the data in a database</li><li>perform obfuscation, low count filtering, etc</li><li>place results in a results queue</li></ul> |
| `app/manger-frontend` | React (Vite) Client SPA for the Manager | |
| `.github` | GitHub Actions | workflows for building and deploying the applications |
| `assets` | Asset source files | Logos etc. |
| `website` | Docusaurus 2 site | The source for https://hdruk.github.io/hutch |

# ⚙️ App Configuration

Configuration guidance for the apps in the stack can be found in the documentation:

- [Manager](https://hdruk.github.io/hutch/docs/users/getting-started/configuration/manager)
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
[![TRE-FX](./website/static/img/tre-fx_logo.svg)][TRE-FX Home]

[HDR UK Home]: https://www.hdruk.ac.uk/
[Cohort Discovery]: https://www.healthdatagateway.org/about/cohort-discovery
[UoN Home]: https://nottingham.ac.uk
[UKRI Home]: https://www.ukri.org/
[DARE UK Home]: https://dareuk.org.uk/
[TRE-FX Home]: https://trefx.uk/
