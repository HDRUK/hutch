"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[458],{8570:(e,t,n)=>{n.d(t,{Zo:()=>m,kt:()=>u});var r=n(79);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function o(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?o(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):o(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function c(e,t){if(null==e)return{};var n,r,a=function(e,t){if(null==e)return{};var n,r,a={},o=Object.keys(e);for(r=0;r<o.length;r++)n=o[r],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(r=0;r<o.length;r++)n=o[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var l=r.createContext({}),s=function(e){var t=r.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},m=function(e){var t=s(e.components);return r.createElement(l.Provider,{value:t},e.children)},d={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},p=r.forwardRef((function(e,t){var n=e.components,a=e.mdxType,o=e.originalType,l=e.parentName,m=c(e,["components","mdxType","originalType","parentName"]),p=s(n),u=a,f=p["".concat(l,".").concat(u)]||p[u]||d[u]||o;return n?r.createElement(f,i(i({ref:t},m),{},{components:n})):r.createElement(f,i({ref:t},m))}));function u(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=n.length,i=new Array(o);i[0]=p;var c={};for(var l in t)hasOwnProperty.call(t,l)&&(c[l]=t[l]);c.originalType=e,c.mdxType="string"==typeof e?e:a,i[1]=c;for(var s=2;s<o;s++)i[s]=n[s];return r.createElement.apply(null,i)}return r.createElement.apply(null,n)}p.displayName="MDXCreateElement"},1026:(e,t,n)=>{n.d(t,{Z:()=>f});var r=n(79),a=n(9841),o=n(3564),i=n(6907);const c="admonition_gmE9",l="admonitionHeading_dXkE",s="admonitionIcon_K5G0",m="admonitionContent_xgCk";const d={note:{infimaClassName:"secondary",iconComponent:function(){return r.createElement("svg",{viewBox:"0 0 14 16"},r.createElement("path",{fillRule:"evenodd",d:"M6.3 5.69a.942.942 0 0 1-.28-.7c0-.28.09-.52.28-.7.19-.18.42-.28.7-.28.28 0 .52.09.7.28.18.19.28.42.28.7 0 .28-.09.52-.28.7a1 1 0 0 1-.7.3c-.28 0-.52-.11-.7-.3zM8 7.99c-.02-.25-.11-.48-.31-.69-.2-.19-.42-.3-.69-.31H6c-.27.02-.48.13-.69.31-.2.2-.3.44-.31.69h1v3c.02.27.11.5.31.69.2.2.42.31.69.31h1c.27 0 .48-.11.69-.31.2-.19.3-.42.31-.69H8V7.98v.01zM7 2.3c-3.14 0-5.7 2.54-5.7 5.68 0 3.14 2.56 5.7 5.7 5.7s5.7-2.55 5.7-5.7c0-3.15-2.56-5.69-5.7-5.69v.01zM7 .98c3.86 0 7 3.14 7 7s-3.14 7-7 7-7-3.12-7-7 3.14-7 7-7z"}))},label:r.createElement(i.Z,{id:"theme.admonition.note",description:"The default label used for the Note admonition (:::note)"},"note")},tip:{infimaClassName:"success",iconComponent:function(){return r.createElement("svg",{viewBox:"0 0 12 16"},r.createElement("path",{fillRule:"evenodd",d:"M6.5 0C3.48 0 1 2.19 1 5c0 .92.55 2.25 1 3 1.34 2.25 1.78 2.78 2 4v1h5v-1c.22-1.22.66-1.75 2-4 .45-.75 1-2.08 1-3 0-2.81-2.48-5-5.5-5zm3.64 7.48c-.25.44-.47.8-.67 1.11-.86 1.41-1.25 2.06-1.45 3.23-.02.05-.02.11-.02.17H5c0-.06 0-.13-.02-.17-.2-1.17-.59-1.83-1.45-3.23-.2-.31-.42-.67-.67-1.11C2.44 6.78 2 5.65 2 5c0-2.2 2.02-4 4.5-4 1.22 0 2.36.42 3.22 1.19C10.55 2.94 11 3.94 11 5c0 .66-.44 1.78-.86 2.48zM4 14h5c-.23 1.14-1.3 2-2.5 2s-2.27-.86-2.5-2z"}))},label:r.createElement(i.Z,{id:"theme.admonition.tip",description:"The default label used for the Tip admonition (:::tip)"},"tip")},danger:{infimaClassName:"danger",iconComponent:function(){return r.createElement("svg",{viewBox:"0 0 12 16"},r.createElement("path",{fillRule:"evenodd",d:"M5.05.31c.81 2.17.41 3.38-.52 4.31C3.55 5.67 1.98 6.45.9 7.98c-1.45 2.05-1.7 6.53 3.53 7.7-2.2-1.16-2.67-4.52-.3-6.61-.61 2.03.53 3.33 1.94 2.86 1.39-.47 2.3.53 2.27 1.67-.02.78-.31 1.44-1.13 1.81 3.42-.59 4.78-3.42 4.78-5.56 0-2.84-2.53-3.22-1.25-5.61-1.52.13-2.03 1.13-1.89 2.75.09 1.08-1.02 1.8-1.86 1.33-.67-.41-.66-1.19-.06-1.78C8.18 5.31 8.68 2.45 5.05.32L5.03.3l.02.01z"}))},label:r.createElement(i.Z,{id:"theme.admonition.danger",description:"The default label used for the Danger admonition (:::danger)"},"danger")},info:{infimaClassName:"info",iconComponent:function(){return r.createElement("svg",{viewBox:"0 0 14 16"},r.createElement("path",{fillRule:"evenodd",d:"M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z"}))},label:r.createElement(i.Z,{id:"theme.admonition.info",description:"The default label used for the Info admonition (:::info)"},"info")},caution:{infimaClassName:"warning",iconComponent:function(){return r.createElement("svg",{viewBox:"0 0 16 16"},r.createElement("path",{fillRule:"evenodd",d:"M8.893 1.5c-.183-.31-.52-.5-.887-.5s-.703.19-.886.5L.138 13.499a.98.98 0 0 0 0 1.001c.193.31.53.501.886.501h13.964c.367 0 .704-.19.877-.5a1.03 1.03 0 0 0 .01-1.002L8.893 1.5zm.133 11.497H6.987v-2.003h2.039v2.003zm0-3.004H6.987V5.987h2.039v4.006z"}))},label:r.createElement(i.Z,{id:"theme.admonition.caution",description:"The default label used for the Caution admonition (:::caution)"},"caution")}},p={secondary:"note",important:"info",success:"tip",warning:"danger"};function u(e){const{mdxAdmonitionTitle:t,rest:n}=function(e){const t=r.Children.toArray(e),n=t.find((e=>{var t;return r.isValidElement(e)&&"mdxAdmonitionTitle"===(null==(t=e.props)?void 0:t.mdxType)})),a=r.createElement(r.Fragment,null,t.filter((e=>e!==n)));return{mdxAdmonitionTitle:n,rest:a}}(e.children);return{...e,title:e.title??t,children:n}}function f(e){const{children:t,type:n,title:i,icon:f}=u(e),h=function(e){const t=p[e]??e;return d[t]||(console.warn(`No admonition config found for admonition type "${t}". Using Info as fallback.`),d.info)}(n),y=i??h.label,{iconComponent:C}=h,g=f??r.createElement(C,null);return r.createElement("div",{className:(0,a.Z)(o.k.common.admonition,o.k.common.admonitionType(e.type),"alert",`alert--${h.infimaClassName}`,c)},r.createElement("div",{className:l},r.createElement("span",{className:s},g),y),r.createElement("div",{className:m},t))}},3268:(e,t,n)=>{n.d(t,{V:()=>o});var r=n(79),a=n(1026);const o=()=>r.createElement(a.Z,{type:"caution",title:"\ud83d\udea7 Under Construction"},r.createElement("p",null,"This section is a work in progress. It may be incomplete and is subject to change."))},1855:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>s,contentTitle:()=>c,default:()=>p,frontMatter:()=>i,metadata:()=>l,toc:()=>m});var r=n(2203),a=(n(79),n(8570)),o=n(3268);const i={sidebar_position:4},c="ROCrates.Net",l={unversionedId:"devs/ROCrates.Net/index",id:"devs/ROCrates.Net/index",title:"ROCrates.Net",description:"ROCrates.Net is a class library written in C# using the .NET Core framework. It aims to provide .NET Core apps with functionality to programmatically build and/or parse RO-Crates. It is implemented as a C# port of the Python library rocrate.",source:"@site/docs/devs/ROCrates.Net/index.md",sourceDirName:"devs/ROCrates.Net",slug:"/devs/ROCrates.Net/",permalink:"/hutch/docs/devs/ROCrates.Net/",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/devs/ROCrates.Net/index.md",tags:[],version:"current",sidebarPosition:4,frontMatter:{sidebar_position:4},sidebar:"devGuide",previous:{title:"Running WfExS",permalink:"/hutch/docs/devs/external-systems/wfexs/running-wfexs"}},s={},m=[{value:"Add <code>ROCrates.Net</code> to your project",id:"add-rocratesnet-to-your-project",level:2},{value:"Creating an RO-Crate",id:"creating-an-ro-crate",level:2},{value:"Consuming an RO-Crate",id:"consuming-an-ro-crate",level:2}],d={toc:m};function p(e){let{components:t,...n}=e;return(0,a.kt)("wrapper",(0,r.Z)({},d,n,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"rocratesnet"},"ROCrates.Net"),(0,a.kt)(o.V,{mdxType:"SectionUnderConstruction"}),(0,a.kt)("p",null,(0,a.kt)("inlineCode",{parentName:"p"},"ROCrates.Net")," is a class library written in C# using the .NET Core framework. It aims to provide .NET Core apps with functionality to programmatically build and/or parse ",(0,a.kt)("a",{parentName:"p",href:"https://www.researchobject.org/ro-crate/"},"RO-Crates"),". It is implemented as a C# port of the Python library ",(0,a.kt)("a",{parentName:"p",href:"https://pypi.org/project/rocrate/"},(0,a.kt)("inlineCode",{parentName:"a"},"rocrate")),". "),(0,a.kt)("h2",{id:"add-rocratesnet-to-your-project"},"Add ",(0,a.kt)("inlineCode",{parentName:"h2"},"ROCrates.Net")," to your project"),(0,a.kt)("p",null,"To add ",(0,a.kt)("inlineCode",{parentName:"p"},"ROCrates.Net")," to your project from your machine, add the following ",(0,a.kt)("inlineCode",{parentName:"p"},"ItemGroup")," to your project's ",(0,a.kt)("inlineCode",{parentName:"p"},".csproj"),", substituting ",(0,a.kt)("inlineCode",{parentName:"p"},"<path/to>")," with the actual path:."),(0,a.kt)("pre",null,(0,a.kt)("code",{parentName:"pre",className:"language-xml"},'<ItemGroup>\n  <ProjectReference Include="<path/to>/ROCrates.Net/ROCrates.Net.csproj" />\n</ItemGroup>\n')),(0,a.kt)("admonition",{title:"TBD",type:"info"},(0,a.kt)("h3",{parentName:"admonition",id:"add-rocratesnet-to-your-project-from-nuget"},"Add ",(0,a.kt)("inlineCode",{parentName:"h3"},"ROCrates.Net")," to your project from NuGet"),(0,a.kt)("pre",{parentName:"admonition"},(0,a.kt)("code",{parentName:"pre",className:"language-xml"},'<ItemGroup>\n  <ProjectReference Include="ROCrates.Net" Version="x.y.z"/>\n</ItemGroup>\n'))),(0,a.kt)("admonition",{type:"caution"},(0,a.kt)("p",{parentName:"admonition"},"It is not currently feature complete with ",(0,a.kt)("inlineCode",{parentName:"p"},"rocrate")," yet!")),(0,a.kt)("h2",{id:"creating-an-ro-crate"},"Creating an RO-Crate"),(0,a.kt)("p",null,"Creating a crate will work similarly to this: ",(0,a.kt)("a",{parentName:"p",href:"https://github.com/ResearchObject/ro-crate-py#creating-an-ro-crate"},"https://github.com/ResearchObject/ro-crate-py#creating-an-ro-crate"),"."),(0,a.kt)("h2",{id:"consuming-an-ro-crate"},"Consuming an RO-Crate"),(0,a.kt)("p",null,"Consuming an crate will work similarly to this: ",(0,a.kt)("a",{parentName:"p",href:"https://github.com/ResearchObject/ro-crate-py#consuming-an-ro-crate"},"https://github.com/ResearchObject/ro-crate-py#consuming-an-ro-crate"),"."))}p.isMDXComponent=!0}}]);