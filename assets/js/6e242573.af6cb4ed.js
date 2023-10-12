"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[752],{38570:(e,t,r)=>{r.d(t,{Zo:()=>u,kt:()=>d});var n=r(70079);function o(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function s(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function i(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?s(Object(r),!0).forEach((function(t){o(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):s(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function c(e,t){if(null==e)return{};var r,n,o=function(e,t){if(null==e)return{};var r,n,o={},s=Object.keys(e);for(n=0;n<s.length;n++)r=s[n],t.indexOf(r)>=0||(o[r]=e[r]);return o}(e,t);if(Object.getOwnPropertySymbols){var s=Object.getOwnPropertySymbols(e);for(n=0;n<s.length;n++)r=s[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(o[r]=e[r])}return o}var a=n.createContext({}),l=function(e){var t=n.useContext(a),r=t;return e&&(r="function"==typeof e?e(t):i(i({},t),e)),r},u=function(e){var t=l(e.components);return n.createElement(a.Provider,{value:t},e.children)},m={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},p=n.forwardRef((function(e,t){var r=e.components,o=e.mdxType,s=e.originalType,a=e.parentName,u=c(e,["components","mdxType","originalType","parentName"]),p=l(r),d=o,f=p["".concat(a,".").concat(d)]||p[d]||m[d]||s;return r?n.createElement(f,i(i({ref:t},u),{},{components:r})):n.createElement(f,i({ref:t},u))}));function d(e,t){var r=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var s=r.length,i=new Array(s);i[0]=p;var c={};for(var a in t)hasOwnProperty.call(t,a)&&(c[a]=t[a]);c.originalType=e,c.mdxType="string"==typeof e?e:o,i[1]=c;for(var l=2;l<s;l++)i[l]=r[l];return n.createElement.apply(null,i)}return n.createElement.apply(null,r)}p.displayName="MDXCreateElement"},30060:(e,t,r)=>{r.d(t,{Z:()=>g});var n=r(70079),o=r(19841),s=r(39085),i=r(46748),c=r(23022),a=r(26839);const l="cardContainer_Ewt8",u="cardTitle_KfEp",m="cardDescription_Fttl";function p(e){let{href:t,children:r}=e;return n.createElement(i.Z,{href:t,className:(0,o.Z)("card padding--lg",l)},r)}function d(e){let{href:t,icon:r,title:s,description:i}=e;return n.createElement(p,{href:t},n.createElement("h2",{className:(0,o.Z)("text--truncate",u),title:s},r," ",s),i&&n.createElement("p",{className:(0,o.Z)("text--truncate",m),title:i},i))}function f(e){let{item:t}=e;const r=(0,s.Wl)(t);return r?n.createElement(d,{href:r,icon:"\ud83d\uddc3\ufe0f",title:t.label,description:(0,a.I)({message:"{count} items",id:"theme.docs.DocCard.categoryDescription",description:"The default description for a category card in the generated index about how many items this category includes"},{count:t.items.length})}):null}function y(e){let{item:t}=e;const r=(0,c.Z)(t.href)?"\ud83d\udcc4\ufe0f":"\ud83d\udd17",o=(0,s.xz)(t.docId??void 0);return n.createElement(d,{href:t.href,icon:r,title:t.label,description:null==o?void 0:o.description})}function h(e){let{item:t}=e;switch(t.type){case"link":return n.createElement(y,{item:t});case"category":return n.createElement(f,{item:t});default:throw new Error(`unknown item type ${JSON.stringify(t)}`)}}function x(e){let{className:t}=e;const r=(0,s.jA)();return n.createElement(g,{items:r.items,className:t})}function g(e){const{items:t,className:r}=e;if(!t)return n.createElement(x,e);const i=(0,s.MN)(t);return n.createElement("section",{className:(0,o.Z)("row",r)},i.map(((e,t)=>n.createElement("article",{key:t,className:"col col--6 margin-bottom--lg"},n.createElement(h,{item:e})))))}},97873:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>a,contentTitle:()=>i,default:()=>m,frontMatter:()=>s,metadata:()=>c,toc:()=>l});var n=r(52203),o=(r(70079),r(38570));r(30060);const s={},i="Nexus",c={unversionedId:"external-systems/nexus/index",id:"external-systems/nexus/index",title:"Nexus",description:"Nexus is a system for building various types of repositories locally, like Git, Docker, Nuget, PyPI and more.",source:"@site/docs/external-systems/nexus/index.md",sourceDirName:"external-systems/nexus",slug:"/external-systems/nexus/",permalink:"/hutch/docs/external-systems/nexus/",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/external-systems/nexus/index.md",tags:[],version:"current",frontMatter:{},sidebar:"docs",previous:{title:"minio-keycloak",permalink:"/hutch/docs/external-systems/minio/minio-keycloak"},next:{title:"Using Nexus",permalink:"/hutch/docs/external-systems/nexus/using_nexus"}},a={},l=[],u={toc:l};function m(e){let{components:t,...r}=e;return(0,o.kt)("wrapper",(0,n.Z)({},u,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"nexus"},"Nexus"),(0,o.kt)("p",null,(0,o.kt)("a",{parentName:"p",href:"https://www.sonatype.com/products/sonatype-nexus-repository"},"Nexus")," is a system for building various types of repositories locally, like Git, Docker, Nuget, PyPI and more."))}m.isMDXComponent=!0}}]);