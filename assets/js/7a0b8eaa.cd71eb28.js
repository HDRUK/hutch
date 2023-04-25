"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[417],{8570:(e,t,n)=>{n.d(t,{Zo:()=>p,kt:()=>m});var r=n(79);function o(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function a(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function i(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?a(Object(n),!0).forEach((function(t){o(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):a(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function l(e,t){if(null==e)return{};var n,r,o=function(e,t){if(null==e)return{};var n,r,o={},a=Object.keys(e);for(r=0;r<a.length;r++)n=a[r],t.indexOf(n)>=0||(o[n]=e[n]);return o}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(r=0;r<a.length;r++)n=a[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(o[n]=e[n])}return o}var s=r.createContext({}),u=function(e){var t=r.useContext(s),n=t;return e&&(n="function"==typeof e?e(t):i(i({},t),e)),n},p=function(e){var t=u(e.components);return r.createElement(s.Provider,{value:t},e.children)},c={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},f=r.forwardRef((function(e,t){var n=e.components,o=e.mdxType,a=e.originalType,s=e.parentName,p=l(e,["components","mdxType","originalType","parentName"]),f=u(n),m=o,w=f["".concat(s,".").concat(m)]||f[m]||c[m]||a;return n?r.createElement(w,i(i({ref:t},p),{},{components:n})):r.createElement(w,i({ref:t},p))}));function m(e,t){var n=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var a=n.length,i=new Array(a);i[0]=f;var l={};for(var s in t)hasOwnProperty.call(t,s)&&(l[s]=t[s]);l.originalType=e,l.mdxType="string"==typeof e?e:o,i[1]=l;for(var u=2;u<a;u++)i[u]=n[u];return r.createElement.apply(null,i)}return r.createElement.apply(null,n)}f.displayName="MDXCreateElement"},9782:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>s,contentTitle:()=>i,default:()=>c,frontMatter:()=>a,metadata:()=>l,toc:()=>u});var r=n(2203),o=(n(79),n(8570));const a={sidebar_position:4},i="Running HutchWorker workflow",l={unversionedId:"devs/external-systems/wfexs/running-test-workflow",id:"devs/external-systems/wfexs/running-test-workflow",title:"Running HutchWorker workflow",description:"Rquest Omop Worker Workflows",source:"@site/docs/devs/external-systems/wfexs/running-test-workflow.md",sourceDirName:"devs/external-systems/wfexs",slug:"/devs/external-systems/wfexs/running-test-workflow",permalink:"/hutch/docs/devs/external-systems/wfexs/running-test-workflow",draft:!1,editUrl:"https://github.com/hdruk/hutch/tree/main/website/docs/devs/external-systems/wfexs/running-test-workflow.md",tags:[],version:"current",sidebarPosition:4,frontMatter:{sidebar_position:4},sidebar:"devGuide",previous:{title:"Running WfExS",permalink:"/hutch/docs/devs/external-systems/wfexs/running-wfexs"},next:{title:"Set up a Ubuntu Linux Environment to Run WfExS",permalink:"/hutch/docs/devs/external-systems/wfexs/wfexs-dev-env"}},s={},u=[{value:"Rquest Omop Worker Workflows",id:"rquest-omop-worker-workflows",level:2},{value:"Configuration",id:"configuration",level:2},{value:"Set up local DB",id:"set-up-local-db",level:3},{value:"Stage file for executing rquest-omop-worker",id:"stage-file-for-executing-rquest-omop-worker",level:3},{value:"WfExS config file",id:"wfexs-config-file",level:3},{value:"Executing the workflow",id:"executing-the-workflow",level:2}],p={toc:u};function c(e){let{components:t,...n}=e;return(0,o.kt)("wrapper",(0,r.Z)({},p,n,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("h1",{id:"running-hutchworker-workflow"},"Running HutchWorker workflow"),(0,o.kt)("h2",{id:"rquest-omop-worker-workflows"},"Rquest Omop Worker Workflows"),(0,o.kt)("p",null,"The rquest-omop-worker workflows can be found ",(0,o.kt)("a",{parentName:"p",href:"https://github.com/HDRUK/hutch/tree/main/workflows"},"here"),"."),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("p",{parentName:"li"},(0,o.kt)("inlineCode",{parentName:"p"},"sec-hutch.cwl"),": Main workflow linked in ",(0,o.kt)("inlineCode",{parentName:"p"},"workflow_id")," in WfExS stage file.")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("p",{parentName:"li"},(0,o.kt)("inlineCode",{parentName:"p"},"sec-hutchx86.cwl"),": Same as above but configured for intel chip")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("p",{parentName:"li"},(0,o.kt)("inlineCode",{parentName:"p"},"rquest-oneshot.cwl"),": CommandLineTool referenced in main workflow.")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("p",{parentName:"li"},(0,o.kt)("inlineCode",{parentName:"p"},"rquest-oneshotx86.cwl"),": Same as above but configured for intel chip"))),(0,o.kt)("p",null,(0,o.kt)("strong",{parentName:"p"},"Note:")," WfExS needs the workflows to be nested as of now, with main workflow linking to the CommandLineTool."),(0,o.kt)("h2",{id:"configuration"},"Configuration"),(0,o.kt)("h3",{id:"set-up-local-db"},"Set up local DB"),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("p",{parentName:"li"},"Create a docker image for a postgres DB - ",(0,o.kt)("a",{parentName:"p",href:"https://hub.docker.com/_/postgres"},"docker postgres image"),". Point the DB volume to the directory with the sample data csv files.")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("p",{parentName:"li"},"Instructions on setting up the sample data once the DB is created can be found ",(0,o.kt)("a",{parentName:"p",href:"/hutch/docs/users/sample-data/omop-53"},"here"),"."))),(0,o.kt)("h3",{id:"stage-file-for-executing-rquest-omop-worker"},"Stage file for executing rquest-omop-worker"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre",className:"language-yaml"},"workflow_id: # URL to workflow \n#choice of github public url, workflow RO-Crate zip archive, github repo URL\nworkflow_config:\n  container: # choice of 'singularity', 'docker', 'podman' or 'none'\n  secure: false\nnickname: 'hutch-rquest-worker' # prefix for the randomly generated nickname\ncacheDir: /path/to/chacheDir\ncrypt4gh: # four random words here\n  key: /path/to/private-key\n  passphrase: \n  pub: /path/to/public-key\noutputs:\n  output_file:\n    c-l-a-s-s: File\n    glob: \"output.json\" # needs to match workflow output\nparams: # parameters needed to run the workflow\n  body: '{...}' # contains rquest query json\n  is_availability: # true or false \n  # Credentials needed to connect to local DB with sample data.\n  db_host:\n  db_name:\n  db_user:\n  db_password:\n")),(0,o.kt)("p",null,(0,o.kt)("strong",{parentName:"p"},"Note:")," If ",(0,o.kt)("inlineCode",{parentName:"p"},"workflow_id")," is set to an absolute path, WfExS expects it to be a path to an RO-Crate."),(0,o.kt)("h3",{id:"wfexs-config-file"},"WfExS config file"),(0,o.kt)("p",null,"An example of a local configuration files can be found ",(0,o.kt)("a",{parentName:"p",href:"https://github.com/inab/WfExS-backend/tree/main/workflow_examples"},"here"),". More specificaly ",(0,o.kt)("inlineCode",{parentName:"p"},"local_config.yaml")," is used to stage the rquest-omop-worker workflow also found ",(0,o.kt)("a",{parentName:"p",href:"/hutch/docs/devs/external-systems/wfexs/config#local-wfexs-configuration"},"here"),"."),(0,o.kt)("h2",{id:"executing-the-workflow"},"Executing the workflow"),(0,o.kt)("p",null,"Once the DB is set up with the sample data you may execute the workflow using these ",(0,o.kt)("a",{parentName:"p",href:"/hutch/docs/devs/external-systems/wfexs/running-wfexs#running-wfexs"},"steps"),". "),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},"Where ",(0,o.kt)("inlineCode",{parentName:"li"},"<path_to_wfexs_config.yaml>")," use the ",(0,o.kt)("a",{parentName:"li",href:"#wfexs-config-file"},"local config"),"."),(0,o.kt)("li",{parentName:"ul"},"Where ",(0,o.kt)("inlineCode",{parentName:"li"},"<stage_file.yaml>")," use the ",(0,o.kt)("a",{parentName:"li",href:"#stage-file-for-executing-rquest-omop-worker"},"wfExS stage file"),".")))}c.isMDXComponent=!0}}]);