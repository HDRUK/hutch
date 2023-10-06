"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[149],{38570:(e,t,n)=>{n.d(t,{Zo:()=>u,kt:()=>c});var a=n(70079);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function l(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function o(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?l(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):l(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function i(e,t){if(null==e)return{};var n,a,r=function(e,t){if(null==e)return{};var n,a,r={},l=Object.keys(e);for(a=0;a<l.length;a++)n=l[a],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var l=Object.getOwnPropertySymbols(e);for(a=0;a<l.length;a++)n=l[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var d=a.createContext({}),p=function(e){var t=a.useContext(d),n=t;return e&&(n="function"==typeof e?e(t):o(o({},t),e)),n},u=function(e){var t=p(e.components);return a.createElement(d.Provider,{value:t},e.children)},s={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},m=a.forwardRef((function(e,t){var n=e.components,r=e.mdxType,l=e.originalType,d=e.parentName,u=i(e,["components","mdxType","originalType","parentName"]),m=p(n),c=r,k=m["".concat(d,".").concat(c)]||m[c]||s[c]||l;return n?a.createElement(k,o(o({ref:t},u),{},{components:n})):a.createElement(k,o({ref:t},u))}));function c(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var l=n.length,o=new Array(l);o[0]=m;var i={};for(var d in t)hasOwnProperty.call(t,d)&&(i[d]=t[d]);i.originalType=e,i.mdxType="string"==typeof e?e:r,o[1]=i;for(var p=2;p<l;p++)o[p]=n[p];return a.createElement.apply(null,o)}return a.createElement.apply(null,n)}m.displayName="MDXCreateElement"},17481:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>d,contentTitle:()=>o,default:()=>s,frontMatter:()=>l,metadata:()=>i,toc:()=>p});var a=n(52203),r=(n(70079),n(38570));const l={},o="Exported Crates",i={unversionedId:"external-systems/wfexs/exported-crates",id:"external-systems/wfexs/exported-crates",title:"Exported Crates",description:"Wfexs will let you export ro-crates of different types (with different contents) at a few points along the way.",source:"@site/docs/external-systems/wfexs/exported-crates.md",sourceDirName:"external-systems/wfexs",slug:"/external-systems/wfexs/exported-crates",permalink:"/hutch/docs/external-systems/wfexs/exported-crates",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/external-systems/wfexs/exported-crates.md",tags:[],version:"current",frontMatter:{},sidebar:"docs",previous:{title:"Set up a Ubuntu Linux Environment to Run WfExS",permalink:"/hutch/docs/external-systems/wfexs/wfexs-dev-env"},next:{title:"Development",permalink:"/hutch/docs/category/development"}},d={},p=[],u={toc:p};function s(e){let{components:t,...n}=e;return(0,r.kt)("wrapper",(0,a.Z)({},u,n,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"exported-crates"},"Exported Crates"),(0,r.kt)("p",null,"Wfexs will let you export ro-crates of different types (with different contents) at a few points along the way."),(0,r.kt)("p",null,"Here is a brief coverage of the different crates you can get."),(0,r.kt)("table",null,(0,r.kt)("thead",{parentName:"table"},(0,r.kt)("tr",{parentName:"thead"},(0,r.kt)("th",{parentName:"tr",align:null},"Command"),(0,r.kt)("th",{parentName:"tr",align:null},"Crate Types"),(0,r.kt)("th",{parentName:"tr",align:null},"Notes"))),(0,r.kt)("tbody",{parentName:"table"},(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"export-stage")),(0,r.kt)("td",{parentName:"tr",align:null},"Staged"),(0,r.kt)("td",{parentName:"tr",align:null},"Must be run on a staged working directory identifier (i.e. after ",(0,r.kt)("inlineCode",{parentName:"td"},"stage")," has occurred)")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"export-stage --full")),(0,r.kt)("td",{parentName:"tr",align:null},"Staged (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"As above")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"export-crate")),(0,r.kt)("td",{parentName:"tr",align:null},"Execute"),(0,r.kt)("td",{parentName:"tr",align:null},"Must be run on an executed working directory (i.e. after ",(0,r.kt)("inlineCode",{parentName:"td"},"offline-exec")," has occurred)")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"export-crate --full")),(0,r.kt)("td",{parentName:"tr",align:null},"Execute (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"As above")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"execute")),(0,r.kt)("td",{parentName:"tr",align:null},"Staged, Execute"),(0,r.kt)("td",{parentName:"tr",align:null},"This also runs ",(0,r.kt)("inlineCode",{parentName:"td"},"stage")," and ",(0,r.kt)("inlineCode",{parentName:"td"},"offline-exec")," for you")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"execute --full")),(0,r.kt)("td",{parentName:"tr",align:null},"Staged (Full), Execute (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"This also runs ",(0,r.kt)("inlineCode",{parentName:"td"},"stage")," and ",(0,r.kt)("inlineCode",{parentName:"td"},"offline-exec")," for you")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"create-prov-crate")),(0,r.kt)("td",{parentName:"tr",align:null},"Provenance"),(0,r.kt)("td",{parentName:"tr",align:null},"Must be run on an executed working directory (i.e. after ",(0,r.kt)("inlineCode",{parentName:"td"},"offline-exec")," has occurred)")),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"create-prov-crate --full")),(0,r.kt)("td",{parentName:"tr",align:null},"Provenance (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"As above")))),(0,r.kt)("table",null,(0,r.kt)("thead",{parentName:"table"},(0,r.kt)("tr",{parentName:"thead"},(0,r.kt)("th",{parentName:"tr",align:null},"Crate"),(0,r.kt)("th",{parentName:"tr",align:null},"Description"),(0,r.kt)("th",{parentName:"tr",align:null},"Commands"))),(0,r.kt)("tbody",{parentName:"table"},(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"Staged"),(0,r.kt)("td",{parentName:"tr",align:null},"the source workflows, packed entry worfklow, crate metadata"),(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"stage")," + ",(0,r.kt)("inlineCode",{parentName:"td"},"export-stage"),", ",(0,r.kt)("inlineCode",{parentName:"td"},"execute"))),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"Staged (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"As above but with inputs and container images"),(0,r.kt)("td",{parentName:"tr",align:null},"as above but with ",(0,r.kt)("inlineCode",{parentName:"td"},"--full"))),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"Execute"),(0,r.kt)("td",{parentName:"tr",align:null},"source + packed worflows, crate metadata"),(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"offline-exec")," + ",(0,r.kt)("inlineCode",{parentName:"td"},"export-crate"),", ",(0,r.kt)("inlineCode",{parentName:"td"},"execute"))),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"Execute (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"As above but with inputs, outputs and container images"),(0,r.kt)("td",{parentName:"tr",align:null},"as above but with ",(0,r.kt)("inlineCode",{parentName:"td"},"--full"))),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"Provenance"),(0,r.kt)("td",{parentName:"tr",align:null},"ATM this seems to be the same as Execute. It probably should have different metadata, but doesn't currently."),(0,r.kt)("td",{parentName:"tr",align:null},(0,r.kt)("inlineCode",{parentName:"td"},"offline-exec")," / ",(0,r.kt)("inlineCode",{parentName:"td"},"execute")," + ",(0,r.kt)("inlineCode",{parentName:"td"},"create-prov-crate"))),(0,r.kt)("tr",{parentName:"tbody"},(0,r.kt)("td",{parentName:"tr",align:null},"Provenance (Full)"),(0,r.kt)("td",{parentName:"tr",align:null},"ATM this seems to be the same as Execute (Full). It probably should have different metadata, but doesn't currently."),(0,r.kt)("td",{parentName:"tr",align:null},"as above with ",(0,r.kt)("inlineCode",{parentName:"td"},"--full"))))))}s.isMDXComponent=!0}}]);