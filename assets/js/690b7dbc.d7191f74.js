"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[4170],{8570:(e,t,r)=>{r.d(t,{Zo:()=>u,kt:()=>d});var n=r(79);function o(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function i(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function a(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?i(Object(r),!0).forEach((function(t){o(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):i(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function s(e,t){if(null==e)return{};var r,n,o=function(e,t){if(null==e)return{};var r,n,o={},i=Object.keys(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||(o[r]=e[r]);return o}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(o[r]=e[r])}return o}var c=n.createContext({}),l=function(e){var t=n.useContext(c),r=t;return e&&(r="function"==typeof e?e(t):a(a({},t),e)),r},u=function(e){var t=l(e.components);return n.createElement(c.Provider,{value:t},e.children)},p={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},m=n.forwardRef((function(e,t){var r=e.components,o=e.mdxType,i=e.originalType,c=e.parentName,u=s(e,["components","mdxType","originalType","parentName"]),m=l(r),d=o,g=m["".concat(c,".").concat(d)]||m[d]||p[d]||i;return r?n.createElement(g,a(a({ref:t},u),{},{components:r})):n.createElement(g,a({ref:t},u))}));function d(e,t){var r=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var i=r.length,a=new Array(i);a[0]=m;var s={};for(var c in t)hasOwnProperty.call(t,c)&&(s[c]=t[c]);s.originalType=e,s.mdxType="string"==typeof e?e:o,a[1]=s;for(var l=2;l<i;l++)a[l]=r[l];return n.createElement.apply(null,a)}return n.createElement.apply(null,r)}m.displayName="MDXCreateElement"},8536:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>c,contentTitle:()=>a,default:()=>p,frontMatter:()=>i,metadata:()=>s,toc:()=>l});var n=r(2203),o=(r(79),r(8570));const i={sidebar_position:4},a="(Optional) Docker Images in a TRE",s={unversionedId:"users/getting-started/configuration/docker-hub-spoof",id:"users/getting-started/configuration/docker-hub-spoof",title:"(Optional) Docker Images in a TRE",description:"Inside a Trusted Research Environment (TRE) there will be no internet access. This means that images cannot be pulled from Docker Hub or any other remote container registry.",source:"@site/docs/users/getting-started/configuration/docker-hub-spoof.md",sourceDirName:"users/getting-started/configuration",slug:"/users/getting-started/configuration/docker-hub-spoof",permalink:"/hutch/docs/users/getting-started/configuration/docker-hub-spoof",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/users/getting-started/configuration/docker-hub-spoof.md",tags:[],version:"current",sidebarPosition:4,frontMatter:{sidebar_position:4},sidebar:"userGuide",previous:{title:"(Optional) WorkflowHub in a TRE",permalink:"/hutch/docs/users/getting-started/configuration/workflowhub-spoof"},next:{title:"Detailed Overview",permalink:"/hutch/docs/category/detailed-overview"}},c={},l=[],u={toc:l};function p(e){let{components:t,...r}=e;return(0,o.kt)("wrapper",(0,n.Z)({},u,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"optional-docker-images-in-a-tre"},"(Optional) Docker Images in a TRE"),(0,o.kt)("p",null,"Inside a Trusted Research Environment (TRE) there will be no internet access. This means that images cannot be pulled from Docker Hub or any other remote container registry."),(0,o.kt)("p",null,(0,o.kt)("strong",{parentName:"p"},"The Docker client cannot be made to point to a custom registry by default"),". This means operations like ",(0,o.kt)("inlineCode",{parentName:"p"},"docker pull postgres")," will always try to get ",(0,o.kt)("inlineCode",{parentName:"p"},"postgres"),", for example, from Docker Hub. To get ",(0,o.kt)("inlineCode",{parentName:"p"},"postgres")," from a custom registry, you need to do ",(0,o.kt)("inlineCode",{parentName:"p"},"docker pull my_custom_registry/postgres"),". Many workflows use Docker images from various registries and it would be time-consuming to manually alter them. Furthermore, doing it programmatically would be impractical."),(0,o.kt)("p",null,"In order to run any images in the TRE without having to alter any image names, ",(0,o.kt)("strong",{parentName:"p"},"you will need to pre-install them into the local environment before disconnecting it from the internet.")," Simply install them locally using ",(0,o.kt)("inlineCode",{parentName:"p"},"docker pull"),". This will save the images locally with names matching what may appear in any workflow."))}p.isMDXComponent=!0}}]);