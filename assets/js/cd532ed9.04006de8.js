"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[474],{38570:(e,n,t)=>{t.d(n,{Zo:()=>p,kt:()=>m});var r=t(70079);function o(e,n,t){return n in e?Object.defineProperty(e,n,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[n]=t,e}function a(e,n){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);n&&(r=r.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),t.push.apply(t,r)}return t}function s(e){for(var n=1;n<arguments.length;n++){var t=null!=arguments[n]?arguments[n]:{};n%2?a(Object(t),!0).forEach((function(n){o(e,n,t[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):a(Object(t)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(t,n))}))}return e}function i(e,n){if(null==e)return{};var t,r,o=function(e,n){if(null==e)return{};var t,r,o={},a=Object.keys(e);for(r=0;r<a.length;r++)t=a[r],n.indexOf(t)>=0||(o[t]=e[t]);return o}(e,n);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(r=0;r<a.length;r++)t=a[r],n.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(o[t]=e[t])}return o}var l=r.createContext({}),u=function(e){var n=r.useContext(l),t=n;return e&&(t="function"==typeof e?e(n):s(s({},n),e)),t},p=function(e){var n=u(e.components);return r.createElement(l.Provider,{value:n},e.children)},c={inlineCode:"code",wrapper:function(e){var n=e.children;return r.createElement(r.Fragment,{},n)}},d=r.forwardRef((function(e,n){var t=e.components,o=e.mdxType,a=e.originalType,l=e.parentName,p=i(e,["components","mdxType","originalType","parentName"]),d=u(t),m=o,y=d["".concat(l,".").concat(m)]||d[m]||c[m]||a;return t?r.createElement(y,s(s({ref:n},p),{},{components:t})):r.createElement(y,s({ref:n},p))}));function m(e,n){var t=arguments,o=n&&n.mdxType;if("string"==typeof e||o){var a=t.length,s=new Array(a);s[0]=d;var i={};for(var l in n)hasOwnProperty.call(n,l)&&(i[l]=n[l]);i.originalType=e,i.mdxType="string"==typeof e?e:o,s[1]=i;for(var u=2;u<a;u++)s[u]=t[u];return r.createElement.apply(null,s)}return r.createElement.apply(null,t)}d.displayName="MDXCreateElement"},22347:(e,n,t)=>{t.r(n),t.d(n,{assets:()=>l,contentTitle:()=>s,default:()=>c,frontMatter:()=>a,metadata:()=>i,toc:()=>u});var r=t(52203),o=(t(70079),t(38570));const a={sidebar_position:1},s="Using Nexus",i={unversionedId:"external-systems/nexus/using_nexus",id:"external-systems/nexus/using_nexus",title:"Using Nexus",description:"Installation",source:"@site/docs/external-systems/nexus/using_nexus.md",sourceDirName:"external-systems/nexus",slug:"/external-systems/nexus/using_nexus",permalink:"/hutch/docs/external-systems/nexus/using_nexus",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/external-systems/nexus/using_nexus.md",tags:[],version:"current",sidebarPosition:1,frontMatter:{sidebar_position:1},sidebar:"docs",previous:{title:"Nexus",permalink:"/hutch/docs/external-systems/nexus/"},next:{title:"File Store on Nexus",permalink:"/hutch/docs/external-systems/nexus/file-store"}},l={},u=[{value:"Installation",id:"installation",level:2},{value:"Running Nexus",id:"running-nexus",level:2},{value:"Getting the admin password",id:"getting-the-admin-password",level:2}],p={toc:u};function c(e){let{components:n,...t}=e;return(0,o.kt)("wrapper",(0,r.Z)({},p,t,{components:n,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"using-nexus"},"Using Nexus"),(0,o.kt)("h2",{id:"installation"},"Installation"),(0,o.kt)("p",null,"The easiest way to install nexus is by using Docker."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-shell"},"docker pull sonatype/nexus3\n")),(0,o.kt)("p",null,"Information about the image as well as other versions are available ",(0,o.kt)("a",{parentName:"p",href:"https://hub.docker.com/r/sonatype/nexus3/"},"here"),"."),(0,o.kt)("admonition",{type:"warning"},(0,o.kt)("p",{parentName:"admonition"},"Nexus only has ",(0,o.kt)("inlineCode",{parentName:"p"},"amd64"),"-based images, so performance may vary depending on your machine's processor.")),(0,o.kt)("h2",{id:"running-nexus"},"Running Nexus"),(0,o.kt)("p",null,"When running Nexus in Docker, you expose a port and map it to port ",(0,o.kt)("inlineCode",{parentName:"p"},"8081")," inside the container. This will let you view the web console at ",(0,o.kt)("inlineCode",{parentName:"p"},"localhost:8081")," in the browser."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-shell"},'# map port 8081 on host to 8081 on the container\ndocker run -p "8081:8081" sonatype/nexus3\n\n# ## OR ##\n\n# map port 1234 on host to 8081 on the container\ndocker run -p "1234:8081" sonatype/nexus3\n')),(0,o.kt)("p",null,"You will also need to expose additional ports for your repository services as well. Suppose you want to add a docker registry to your Nexus, you could map port ",(0,o.kt)("inlineCode",{parentName:"p"},"8082"),". Additionally you could map ",(0,o.kt)("inlineCode",{parentName:"p"},"8083")," for a git repo on your Nexus."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-shell"},'docker run -p "8081:8081" -p "8082:8082" -p "8083:8083" sonatype/nexus3\n')),(0,o.kt)("p",null,"If you use ",(0,o.kt)("inlineCode",{parentName:"p"},"docker-compose"),", a Nexus service might look like this:"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-yaml"},'nexus:\n  image: sonatype/nexus3\n  restart: always\n  ports:\n    - "8081:8081" # web portal port\n    - "8082:8082" # port for the docker registry\n    - "8083:8083" # port for the git system\n')),(0,o.kt)("h2",{id:"getting-the-admin-password"},"Getting the admin password"),(0,o.kt)("p",null,"The admin password can be obtained by running the following command in the terminal."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-shell"},"docker exec nexus cat /nexus-data/admin.password \n")))}c.isMDXComponent=!0}}]);