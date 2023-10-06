"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[329],{38570:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>h});var r=n(70079);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,r,a=function(e,t){if(null==e)return{};var n,r,a={},o=Object.keys(e);for(r=0;r<o.length;r++)n=o[r],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(r=0;r<o.length;r++)n=o[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var l=r.createContext({}),u=function(e){var t=r.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},p=function(e){var t=u(e.components);return r.createElement(l.Provider,{value:t},e.children)},c={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},d=r.forwardRef((function(e,t){var n=e.components,a=e.mdxType,o=e.originalType,l=e.parentName,p=s(e,["components","mdxType","originalType","parentName"]),d=u(n),h=a,g=d["".concat(l,".").concat(h)]||d[h]||c[h]||o;return n?r.createElement(g,i(i({ref:t},p),{},{components:n})):r.createElement(g,i({ref:t},p))}));function h(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=n.length,i=new Array(o);i[0]=d;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s.mdxType="string"==typeof e?e:a,i[1]=s;for(var u=2;u<o;u++)i[u]=n[u];return r.createElement.apply(null,i)}return r.createElement.apply(null,n)}d.displayName="MDXCreateElement"},23056:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>l,contentTitle:()=>i,default:()=>c,frontMatter:()=>o,metadata:()=>s,toc:()=>u});var r=n(52203),a=(n(70079),n(38570));const o={sidebar_position:1},i="Introduction",s={unversionedId:"getting-started/index",id:"getting-started/index",title:"Introduction",description:"Hutch is an open-source tool that enables federated activities on your data. Third parties can run analyses, train machine learning models and much more against your data without it ever leaving your custody.",source:"@site/docs/getting-started/index.md",sourceDirName:"getting-started",slug:"/getting-started/",permalink:"/hutch/docs/getting-started/",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/getting-started/index.md",tags:[],version:"current",sidebarPosition:1,frontMatter:{sidebar_position:1},sidebar:"docs",previous:{title:"Getting Started",permalink:"/hutch/docs/category/getting-started"},next:{title:"Installation",permalink:"/hutch/docs/getting-started/installation/"}},l={},u=[{value:"The Architecture of Hutch",id:"the-architecture-of-hutch",level:2},{value:"The Submission Layer",id:"the-submission-layer",level:3},{value:"The TRE Agent",id:"the-tre-agent",level:3},{value:"The Executing Agent (e.g. Hutch)",id:"the-executing-agent-eg-hutch",level:3},{value:"Intermediary Store",id:"intermediary-store",level:3},{value:"Hutch Implementation Specifics",id:"hutch-implementation-specifics",level:2},{value:"Accepting jobs",id:"accepting-jobs",level:3},{value:"Executing jobs",id:"executing-jobs",level:3},{value:"Status and Results",id:"status-and-results",level:3}],p={toc:u};function c(e){let{components:t,...n}=e;return(0,a.kt)("wrapper",(0,r.Z)({},p,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"introduction"},"Introduction"),(0,a.kt)("p",null,"Hutch is an open-source tool that enables federated activities on your data. Third parties can run analyses, train machine learning models and much more against your data without it ever leaving your custody."),(0,a.kt)("p",null,"Hutch is ideal for Trusted Research Environments (TREs) or Secure Data Environments (SDEs)."),(0,a.kt)("p",null,"\u2705 No need to grant any external systems or people access to your data.",(0,a.kt)("br",null),"\n\u2705 No need to share your data with any other parties.",(0,a.kt)("br",null),"\n\u2705 No need to allow inbound requests to your network."),(0,a.kt)("h2",{id:"the-architecture-of-hutch"},"The Architecture of Hutch"),(0,a.kt)("p",null,'Hutch is part of an "application stack" that was defined by the TRE-FX Project. Hutch itself provides an implementation of the "Executing Agent" module of that stack, interacting in a standard with the other modules, and leveraging existing tools to achieve its functionality.'),(0,a.kt)("p",null,"This affords it a great deal of flexibility in terms of how users may want to run it in different setups and enables it to be easily extended to support new formats or functionality."),(0,a.kt)("p",null,"The main components of the TRE-FX stack are as follows:"),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},"The Submission Layer"),(0,a.kt)("li",{parentName:"ul"},"The TRE Agent (Hutch interacts with this)"),(0,a.kt)("li",{parentName:"ul"},"The Executing Agent (Hutch fulfills this module)"),(0,a.kt)("li",{parentName:"ul"},"An Intermediary Store (Hutch interacts with this)")),(0,a.kt)("p",null,'Additionally, depending on how (or if!) you implement other areas of the stack, and what your needs are regarding "airgapped" environments, you may require your own:'),(0,a.kt)("ul",null,(0,a.kt)("li",{parentName:"ul"},"Container registry"),(0,a.kt)("li",{parentName:"ul"},"Workflow repository")),(0,a.kt)("h3",{id:"the-submission-layer"},"The Submission Layer"),(0,a.kt)("p",null,"The submission layer is an interoperable part of the stack which can accept requests from various federated activity providers in the form of an RO-Crate. It sits just outside the TRE/SDE, still only allowing outbound requests for activities to be run. External services cannot directly put jobs into the TRE/SDE to be run."),(0,a.kt)("p",null,'The submission layer aims to reduce or prevent "vendor lock-in" whereby you enable federated activities only with researchers with access to one specific product. This increases the reach of your data in the research community.'),(0,a.kt)("h3",{id:"the-tre-agent"},"The TRE Agent"),(0,a.kt)("p",null,"The TRE Agent sits inside the TRE/SDE to verify and approve incoming jobs and facilitate the approval of data egress within existing TRE workflows.\nIt is the only module within the stack that is allowed outside communication (specifically with the Submission Layer) from inside the TRE."),(0,a.kt)("h3",{id:"the-executing-agent-eg-hutch"},"The Executing Agent (e.g. Hutch)"),(0,a.kt)("p",null,"The Executing Agent runs workflow jobs passed to it from the TRE Agent. The RO-Crate in the request is unpacked and, in the case of Hutch, executed using WfExS. Upon completion of the job, outputs are placed in the Intermediary Store and the TRE Agent is notified that they are ready to be inspected for data egress approval. If approved, the results are merged back into the original RO-Crate and returned to the Intermediary Store ready for egress."),(0,a.kt)("h3",{id:"intermediary-store"},"Intermediary Store"),(0,a.kt)("p",null,"This store serves as a place to put the crates for job requests, outputs of executions, and final results crates. Essentially any transfer between the TRE Agent and the Executing Agent (Hutch) can be performed via this store."),(0,a.kt)("h2",{id:"hutch-implementation-specifics"},"Hutch Implementation Specifics"),(0,a.kt)("p",null,"Hutch implements the ",(0,a.kt)("strong",{parentName:"p"},"Executing Agent")," part of the stack, and interacts with the ",(0,a.kt)("strong",{parentName:"p"},"TRE Agent")," and ",(0,a.kt)("strong",{parentName:"p"},"Intermediary Store"),", as well as optional peripheral components."),(0,a.kt)("p",null,"Hutch itself is a .NET application, running ASP.NET Core to allow it to provide a REST API for certain interactions."),(0,a.kt)("h3",{id:"accepting-jobs"},"Accepting jobs"),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," expects a ",(0,a.kt)("strong",{parentName:"p"},"TRE Agent")," to POST jobs to a jobs endpoint over HTTPS."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," will verify that incoming jobs meet the ",(0,a.kt)("strong",{parentName:"p"},"TRE-FX 5 Safes RO-Crate Profile")," in the expected state, and if valid will execute the requested workflow."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," depends on a local ",(0,a.kt)("strong",{parentName:"p"},"RabbitMQ")," instance for managing its own jobs queue."),(0,a.kt)("h3",{id:"executing-jobs"},"Executing jobs"),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," is capable of executing workflows contained within the crate, fetching workflows from a public source such as ",(0,a.kt)("strong",{parentName:"p"},"WorkflowHub"),", or (as per TRE-FX requirements to support airgapped environments) fetching only approved workflows from an HTTP source within the airgapped environment. Workflows are expected to be in a ",(0,a.kt)("strong",{parentName:"p"},"Workflow Profile RO-Crate"),"."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," executes ",(0,a.kt)("strong",{parentName:"p"},"CWL")," or ",(0,a.kt)("strong",{parentName:"p"},"Nextflow")," workflows using ",(0,a.kt)("strong",{parentName:"p"},"WfExS"),". ",(0,a.kt)("strong",{parentName:"p"},"WfExS")," is an open source python application that supports CWL and Nextflow workflows, and supports running those workflows in Containers via a number of different container engines such as ",(0,a.kt)("strong",{parentName:"p"},"docker")," and ",(0,a.kt)("strong",{parentName:"p"},"podman"),"."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," uses ",(0,a.kt)("strong",{parentName:"p"},"WfExS")," with ",(0,a.kt)("strong",{parentName:"p"},"podman")," due to its better support for airgapped environments. ",(0,a.kt)("strong",{parentName:"p"},"Podman")," supports fetching Container Images (such as those for tools used by workflows) from local registries inside an airgapped environment."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch"),"'s documentation uses ",(0,a.kt)("strong",{parentName:"p"},"Sonatype Nexus")," to fill the role of a local airgapped worflow store and container registry, but these can be fulfilled by other tools as desired."),(0,a.kt)("h3",{id:"status-and-results"},"Status and Results"),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," interacts with the ",(0,a.kt)("strong",{parentName:"p"},"TRE Agent"),"'s REST API to provide status updates."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," provides workflow outputs to the ",(0,a.kt)("strong",{parentName:"p"},"TRE Agent")," via REST API / the ",(0,a.kt)("strong",{parentName:"p"},"Intermediary Store"),", to enable disclosure checking to approve outputs for egress."),(0,a.kt)("p",null,(0,a.kt)("strong",{parentName:"p"},"Hutch")," today can use the ",(0,a.kt)("strong",{parentName:"p"},"AWS S3 API")," (e.g. with a ",(0,a.kt)("strong",{parentName:"p"},"Minio")," server), or a mounted filesystem path, as an ",(0,a.kt)("strong",{parentName:"p"},"Intermediary Store"),"."))}c.isMDXComponent=!0}}]);