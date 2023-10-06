"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[577],{38570:(e,t,n)=>{n.d(t,{Zo:()=>u,kt:()=>m});var r=n(70079);function o(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function a(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?a(Object(n),!0).forEach((function(t){o(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):a(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function l(e,t){if(null==e)return{};var n,r,o=function(e,t){if(null==e)return{};var n,r,o={},a=Object.keys(e);for(r=0;r<a.length;r++)n=a[r],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(r=0;r<a.length;r++)n=a[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}var p=r.createContext({}),s=function(e){var t=r.useContext(p),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},u=function(e){var t=s(e.components);return r.createElement(p.Provider,{value:t},e.children)},c={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},d=r.forwardRef((function(e,t){var n=e.components,o=e.mdxType,a=e.originalType,p=e.parentName,u=l(e,["components","mdxType","originalType","parentName"]),d=s(n),m=o,f=d["".concat(p,".").concat(m)]||d[m]||c[m]||a;return n?r.createElement(f,i(i({ref:t},u),{},{components:n})):r.createElement(f,i({ref:t},u))}));function m(e,t){var n=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var a=n.length,i=new Array(a);i[0]=d;var l={};for(var p in t)hasOwnProperty.call(t,p)&&(l[p]=t[p]);l.originalType=e,l.mdxType="string"==typeof e?e:o,i[1]=l;for(var s=2;s<a;s++)i[s]=n[s];return r.createElement.apply(null,i)}return r.createElement.apply(null,n)}d.displayName="MDXCreateElement"},11322:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>p,contentTitle:()=>i,default:()=>c,frontMatter:()=>a,metadata:()=>l,toc:()=>s});var r=n(52203),o=(n(70079),n(38570));const a={sidebar_position:2},i="Getting Started",l={unversionedId:"development/index",id:"development/index",title:"Getting Started",description:"Here's how to run a local development stack, and guidance on developing different specific parts of the stack",source:"@site/docs/development/index.md",sourceDirName:"development",slug:"/development/",permalink:"/hutch/docs/development/",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/development/index.md",tags:[],version:"current",sidebarPosition:2,frontMatter:{sidebar_position:2},sidebar:"docs",previous:{title:"Development",permalink:"/hutch/docs/category/development"},next:{title:"TRE-FX",permalink:"/hutch/docs/category/tre-fx"}},p={},s=[{value:"Prerequisites",id:"prerequisites",level:2},{value:"Optional for running workflows end to end",id:"optional-for-running-workflows-end-to-end",level:3},{value:"For developing the documentation website",id:"for-developing-the-documentation-website",level:3}],u={toc:s};function c(e){let{components:t,...n}=e;return(0,o.kt)("wrapper",(0,r.Z)({},u,n,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"getting-started"},"Getting Started"),(0,o.kt)("p",null,"Here's how to run a local development stack, and guidance on developing different specific parts of the stack"),(0,o.kt)("h2",{id:"prerequisites"},"Prerequisites"),(0,o.kt)("ol",null,(0,o.kt)("li",{parentName:"ol"},(0,o.kt)("strong",{parentName:"li"},".NET SDK")," ",(0,o.kt)("inlineCode",{parentName:"li"},"7.x")),(0,o.kt)("li",{parentName:"ol"},"A ",(0,o.kt)("strong",{parentName:"li"},"RabbitMQ")," instance"),(0,o.kt)("li",{parentName:"ol"},"A ",(0,o.kt)("strong",{parentName:"li"},"WfExS")," environment (if you need to be actually running workflows)")),(0,o.kt)("h3",{id:"optional-for-running-workflows-end-to-end"},"Optional for running workflows end to end"),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("strong",{parentName:"li"},"Podman")," - typically Hutch will use WfExS with Podman rather than Docker for airgapped environments"),(0,o.kt)("li",{parentName:"ul"},"A Local Container Registry for airgapped use - e.g. ",(0,o.kt)("strong",{parentName:"li"},"Sonatype Nexus")),(0,o.kt)("li",{parentName:"ul"},"A Local Workflow Store for airgapped use - e.g. ",(0,o.kt)("strong",{parentName:"li"},"Sonatype Nexus"),(0,o.kt)("ul",{parentName:"li"},(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("strong",{parentName:"li"},"Nginx")," for proxying workflow URLs to the airgapped store"))),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("strong",{parentName:"li"},"Minio")," as an ",(0,o.kt)("strong",{parentName:"li"},"Intermediary Store")),(0,o.kt)("li",{parentName:"ul"},"Data sources for workflows - e.g. a ",(0,o.kt)("strong",{parentName:"li"},"PostgreSQL")," DB")),(0,o.kt)("h3",{id:"for-developing-the-documentation-website"},"For developing the documentation website"),(0,o.kt)("ol",null,(0,o.kt)("li",{parentName:"ol"},(0,o.kt)("strong",{parentName:"li"},"Node.js")," ",(0,o.kt)("inlineCode",{parentName:"li"},"16.10+")),(0,o.kt)("li",{parentName:"ol"},(0,o.kt)("strong",{parentName:"li"},"Corepack")," enabled")),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},"Just run ",(0,o.kt)("inlineCode",{parentName:"li"},"corepack enable")," in a terminal with node in the PATH")),(0,o.kt)("blockquote",null,(0,o.kt)("p",{parentName:"blockquote"},"\u2139\ufe0f"),(0,o.kt)("p",{parentName:"blockquote"},"The provided ",(0,o.kt)("inlineCode",{parentName:"p"},"docker-compose.yml")," provides suitable development instances of many peripheral services:")),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},"Postgres"),(0,o.kt)("li",{parentName:"ul"},"RabbitMQ"),(0,o.kt)("li",{parentName:"ul"},"Nexus"),(0,o.kt)("li",{parentName:"ul"},"Minio"),(0,o.kt)("li",{parentName:"ul"},"Nginx")))}c.isMDXComponent=!0}}]);