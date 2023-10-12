"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[53],{1109:e=>{e.exports=JSON.parse('{"pluginId":"default","version":"current","label":"Next","banner":null,"badge":false,"noIndex":false,"className":"docs-version-current","isLast":true,"docsSidebars":{"docs":[{"type":"category","label":"Getting Started","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Introduction","href":"/hutch/docs/getting-started/","docId":"getting-started/index"},{"type":"category","label":"Installation","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Hutch Agent","href":"/hutch/docs/getting-started/installation/agent","docId":"getting-started/installation/agent"}],"href":"/hutch/docs/getting-started/installation/"},{"type":"category","label":"Configuration","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Hutch Agent","href":"/hutch/docs/getting-started/configuration/agent","docId":"getting-started/configuration/agent"},{"type":"link","label":"WorkflowHub in a TRE","href":"/hutch/docs/getting-started/configuration/workflowhub-spoof","docId":"getting-started/configuration/workflowhub-spoof"},{"type":"link","label":"Docker Images in a TRE","href":"/hutch/docs/getting-started/configuration/docker-hub-spoof","docId":"getting-started/configuration/docker-hub-spoof"}],"href":"/hutch/docs/category/configuration"}],"href":"/hutch/docs/category/getting-started"},{"type":"category","label":"External Systems","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"index","href":"/hutch/docs/external-systems/keycloak/","docId":"external-systems/keycloak/index"},{"type":"category","label":"MinIO","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Using MinIO","href":"/hutch/docs/external-systems/minio/using_minio","docId":"external-systems/minio/using_minio"},{"type":"link","label":"minio-keycloak","href":"/hutch/docs/external-systems/minio/minio-keycloak","docId":"external-systems/minio/minio-keycloak"}],"href":"/hutch/docs/external-systems/minio/"},{"type":"category","label":"Nexus","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Using Nexus","href":"/hutch/docs/external-systems/nexus/using_nexus","docId":"external-systems/nexus/using_nexus"},{"type":"link","label":"File Store on Nexus","href":"/hutch/docs/external-systems/nexus/file-store","docId":"external-systems/nexus/file-store"}],"href":"/hutch/docs/external-systems/nexus/"},{"type":"category","label":"WfExS","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Installing WfExS","href":"/hutch/docs/external-systems/wfexs/installing-wfexs","docId":"external-systems/wfexs/installing-wfexs"},{"type":"link","label":"Configuration","href":"/hutch/docs/external-systems/wfexs/config","docId":"external-systems/wfexs/config"},{"type":"link","label":"Running WfExS","href":"/hutch/docs/external-systems/wfexs/running-wfexs","docId":"external-systems/wfexs/running-wfexs"},{"type":"link","label":"Set up a Ubuntu Linux Environment to Run WfExS","href":"/hutch/docs/external-systems/wfexs/wfexs-dev-env","docId":"external-systems/wfexs/wfexs-dev-env"},{"type":"link","label":"Exported Crates","href":"/hutch/docs/external-systems/wfexs/exported-crates","docId":"external-systems/wfexs/exported-crates"}],"href":"/hutch/docs/external-systems/wfexs/"}],"href":"/hutch/docs/category/external-systems"},{"type":"category","label":"Development","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"Getting Started","href":"/hutch/docs/development/","docId":"development/index"}],"href":"/hutch/docs/category/development"},{"type":"category","label":"TRE-FX","collapsible":true,"collapsed":true,"items":[{"type":"link","label":"TRE-FX Deployment Notes","href":"/hutch/docs/tre-fx/deployment-notes","docId":"tre-fx/deployment-notes"}],"href":"/hutch/docs/category/tre-fx"}]},"docs":{"development/index":{"id":"development/index","title":"Getting Started","description":"Here\'s how to run a local development stack, and guidance on developing different specific parts of the stack","sidebar":"docs"},"external-systems/keycloak/index":{"id":"external-systems/keycloak/index","title":"index","description":"Keycloak is a popular Open ID Connect (OIDC) Identity Provider.","sidebar":"docs"},"external-systems/minio/index":{"id":"external-systems/minio/index","title":"MinIO","description":"MinIO is a system for S3 compatible object storage. There are SDKs for MinIO in various languages like .NET, Javascript, Python, etc.","sidebar":"docs"},"external-systems/minio/minio-keycloak":{"id":"external-systems/minio/minio-keycloak","title":"minio-keycloak","description":"You can connect Minio to Keycloak as an additional authentication source.","sidebar":"docs"},"external-systems/minio/using_minio":{"id":"external-systems/minio/using_minio","title":"Using MinIO","description":"Installation","sidebar":"docs"},"external-systems/nexus/file-store":{"id":"external-systems/nexus/file-store","title":"File Store on Nexus","description":"Making a hosted File Store","sidebar":"docs"},"external-systems/nexus/index":{"id":"external-systems/nexus/index","title":"Nexus","description":"Nexus is a system for building various types of repositories locally, like Git, Docker, Nuget, PyPI and more.","sidebar":"docs"},"external-systems/nexus/using_nexus":{"id":"external-systems/nexus/using_nexus","title":"Using Nexus","description":"Installation","sidebar":"docs"},"external-systems/wfexs/config":{"id":"external-systems/wfexs/config","title":"Configuration","description":"Local WfExS Configuration","sidebar":"docs"},"external-systems/wfexs/exported-crates":{"id":"external-systems/wfexs/exported-crates","title":"Exported Crates","description":"Wfexs will let you export ro-crates of different types (with different contents) at a few points along the way.","sidebar":"docs"},"external-systems/wfexs/index":{"id":"external-systems/wfexs/index","title":"WfExS","description":"Hutch uses the Workflow Execution Service (WfExS) backend for running workflows on user provided inputs and data. WfExS can be found at https://github.com/inab/WfExS-backend.","sidebar":"docs"},"external-systems/wfexs/installing-wfexs":{"id":"external-systems/wfexs/installing-wfexs","title":"Installing WfExS","description":"System requirements","sidebar":"docs"},"external-systems/wfexs/running-wfexs":{"id":"external-systems/wfexs/running-wfexs","title":"Running WfExS","description":"For a full description of all functions offered by WfExS, refer to the README at https://github.com/inab/WfExS-backend.","sidebar":"docs"},"external-systems/wfexs/wfexs-dev-env":{"id":"external-systems/wfexs/wfexs-dev-env","title":"Set up a Ubuntu Linux Environment to Run WfExS","description":"In the hutch monorepo there is an Ansible playbook which you can use to quickly build an Ubuntu Linux environment for running WfExS.","sidebar":"docs"},"getting-started/configuration/agent":{"id":"getting-started/configuration/agent","title":"Hutch Agent","description":"Hutch can be configured using the following source in the usual .NET way, in order of precedence:","sidebar":"docs"},"getting-started/configuration/docker-hub-spoof":{"id":"getting-started/configuration/docker-hub-spoof","title":"Docker Images in a TRE","description":"This page assumes you are using Ubuntu Linux as your OS and that you will have root or sudo privileges.","sidebar":"docs"},"getting-started/configuration/workflowhub-spoof":{"id":"getting-started/configuration/workflowhub-spoof","title":"WorkflowHub in a TRE","description":"This page assumes you are using Ubuntu Linux as your OS and that you will have root or sudo privileges.","sidebar":"docs"},"getting-started/index":{"id":"getting-started/index","title":"Introduction","description":"Hutch is an open-source tool that enables federated activities on your data. Third parties can run analyses, train machine learning models and much more against your data without it ever leaving your custody.","sidebar":"docs"},"getting-started/installation/agent":{"id":"getting-started/installation/agent","title":"Hutch Agent","description":"The Hutch Agent is a .NET application written in C#.","sidebar":"docs"},"getting-started/installation/index":{"id":"getting-started/installation/index","title":"Installation","description":"To run the Hutch application stack, there are four components that need installing.","sidebar":"docs"},"tre-fx/deployment-notes":{"id":"tre-fx/deployment-notes","title":"TRE-FX Deployment Notes","description":"This section contains notes specific to the deployment of testing/showcase environments for the TRE-FX project.","sidebar":"docs"}}}')}}]);