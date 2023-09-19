/*! For license information please see 47.3feb5381.js.LICENSE.txt */
"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[47],{6506:(e,t,r)=>{r.d(t,{zx:()=>h});var n=r(79);function a(...e){return t=>{e.forEach((e=>{!function(e,t){if(null!=e)if("function"!=typeof e)try{e.current=t}catch(r){throw new Error(`Cannot assign value '${t}' to ref '${e}'`)}else e(t)}(e,t)}))}}function o(...e){return(0,n.useMemo)((()=>a(...e)),e)}var i=r(7297),c=r(8026),l=r(2548),s=r(2463),u=(...e)=>e.filter(Boolean).join(" "),f=e=>e?"":void 0,[p,m]=(0,l.k)({strict:!1,name:"ButtonGroupContext"});function d(e){const{children:t,className:r,...a}=e,o=(0,n.isValidElement)(t)?(0,n.cloneElement)(t,{"aria-hidden":!0,focusable:!1}):t,c=u("chakra-button__icon",r);return n.createElement(i.m$.span,{display:"inline-flex",alignSelf:"center",flexShrink:0,...a,className:c},o)}function y(e){const{label:t,placement:r,spacing:a="0.5rem",children:o=n.createElement(s.$,{color:"currentColor",width:"1em",height:"1em"}),className:c,__css:l,...f}=e,p=u("chakra-button__spinner",c),m="start"===r?"marginEnd":"marginStart",d=(0,n.useMemo)((()=>({display:"flex",alignItems:"center",position:t?"relative":"absolute",[m]:t?a:0,fontSize:"1em",lineHeight:"normal",...l})),[l,t,m,a]);return n.createElement(i.m$.div,{className:p,...f,__css:d},o)}d.displayName="ButtonIcon",y.displayName="ButtonSpinner";var h=(0,i.Gp)(((e,t)=>{const r=m(),a=(0,i.mq)("Button",{...r,...e}),{isDisabled:l=(null==r?void 0:r.isDisabled),isLoading:s,isActive:p,children:d,leftIcon:h,rightIcon:v,loadingText:b,iconSpacing:_="0.5rem",type:x,spinner:E,spinnerPlacement:w="start",className:S,as:k,...N}=(0,c.Lr)(e),C=(0,n.useMemo)((()=>{const e={...null==a?void 0:a._focus,zIndex:1};return{display:"inline-flex",appearance:"none",alignItems:"center",justifyContent:"center",userSelect:"none",position:"relative",whiteSpace:"nowrap",verticalAlign:"middle",outline:"none",...a,...!!r&&{_focus:e}}}),[a,r]),{ref:$,type:j}=function(e){const[t,r]=(0,n.useState)(!e);return{ref:(0,n.useCallback)((e=>{e&&r("BUTTON"===e.tagName)}),[]),type:t?"button":void 0}}(k),O={rightIcon:v,leftIcon:h,iconSpacing:_,children:d};return n.createElement(i.m$.button,{disabled:l||s,ref:o(t,$),as:k,type:x??j,"data-active":f(p),"data-loading":f(s),__css:C,className:u("chakra-button",S),...N},s&&"start"===w&&n.createElement(y,{className:"chakra-button__spinner--start",label:b,placement:"start",spacing:_},E),s?b||n.createElement(i.m$.span,{opacity:0},n.createElement(g,{...O})):n.createElement(g,{...O}),s&&"end"===w&&n.createElement(y,{className:"chakra-button__spinner--end",label:b,placement:"end",spacing:_},E))}));function g(e){const{leftIcon:t,rightIcon:r,children:a,iconSpacing:o}=e;return n.createElement(n.Fragment,null,t&&n.createElement(d,{marginEnd:o},t),a,r&&n.createElement(d,{marginStart:o},r))}h.displayName="Button",(0,i.Gp)((function(e,t){const{size:r,colorScheme:a,variant:o,className:c,spacing:l="0.5rem",isAttached:s,isDisabled:f,...m}=e,d=u("chakra-button__group",c),y=(0,n.useMemo)((()=>({size:r,colorScheme:a,variant:o,isDisabled:f})),[r,a,o,f]);let h={display:"inline-flex"};return h=s?{...h,"> *:first-of-type:not(:last-of-type)":{borderEndRadius:0},"> *:not(:first-of-type):not(:last-of-type)":{borderRadius:0},"> *:not(:first-of-type):last-of-type":{borderStartRadius:0}}:{...h,"& > *:not(style) ~ *:not(style)":{marginStart:l}},n.createElement(p,{value:y},n.createElement(i.m$.div,{ref:t,role:"group",__css:h,className:d,"data-attached":s?"":void 0,...m}))})).displayName="ButtonGroup",(0,i.Gp)(((e,t)=>{const{icon:r,children:a,isRound:o,"aria-label":i,...c}=e,l=r||a,s=(0,n.isValidElement)(l)?(0,n.cloneElement)(l,{"aria-hidden":!0,focusable:!1}):null;return n.createElement(h,{padding:"0",borderRadius:o?"full":void 0,ref:t,"aria-label":i,...c},s)})).displayName="IconButton"},2680:(e,t,r)=>{r.d(t,{kC:()=>j,Ug:()=>z,X6:()=>I,rU:()=>R,xv:()=>D,gC:()=>q});var n=r(79),a=r(7297);function o(e){const t=typeof e;return null!=e&&("object"===t||"function"===t)&&!Array.isArray(e)}Object.freeze(["base","sm","md","lg","xl","2xl"]);function i(e,t){return Array.isArray(e)?e.map((e=>null===e?null:t(e))):o(e)?Object.keys(e).reduce(((r,n)=>(r[n]=t(e[n]),r)),{}):null!=e?t(e):null}var c=r(6684),l=r(8026);function s(e){const t=Object.assign({},e);for(let r in t)void 0===t[r]&&delete t[r];return t}var u,f,p,m=r(6099),d=r(2548),y=Object.create,h=Object.defineProperty,g=Object.getOwnPropertyDescriptor,v=Object.getOwnPropertyNames,b=Object.getPrototypeOf,_=Object.prototype.hasOwnProperty,x=(e,t)=>function(){return t||(0,e[v(e)[0]])((t={exports:{}}).exports,t),t.exports},E=x({"../../node_modules/.pnpm/react@18.2.0/node_modules/react/cjs/react.production.min.js"(e){var t=Symbol.for("react.element"),r=Symbol.for("react.portal"),n=Symbol.for("react.fragment"),a=Symbol.for("react.strict_mode"),o=Symbol.for("react.profiler"),i=Symbol.for("react.provider"),c=Symbol.for("react.context"),l=Symbol.for("react.forward_ref"),s=Symbol.for("react.suspense"),u=Symbol.for("react.memo"),f=Symbol.for("react.lazy"),p=Symbol.iterator;var m={isMounted:function(){return!1},enqueueForceUpdate:function(){},enqueueReplaceState:function(){},enqueueSetState:function(){}},d=Object.assign,y={};function h(e,t,r){this.props=e,this.context=t,this.refs=y,this.updater=r||m}function g(){}function v(e,t,r){this.props=e,this.context=t,this.refs=y,this.updater=r||m}h.prototype.isReactComponent={},h.prototype.setState=function(e,t){if("object"!=typeof e&&"function"!=typeof e&&null!=e)throw Error("setState(...): takes an object of state variables to update or a function which returns an object of state variables.");this.updater.enqueueSetState(this,e,t,"setState")},h.prototype.forceUpdate=function(e){this.updater.enqueueForceUpdate(this,e,"forceUpdate")},g.prototype=h.prototype;var b=v.prototype=new g;b.constructor=v,d(b,h.prototype),b.isPureReactComponent=!0;var _=Array.isArray,x=Object.prototype.hasOwnProperty,E={current:null},w={key:!0,ref:!0,__self:!0,__source:!0};function S(e,r,n){var a,o={},i=null,c=null;if(null!=r)for(a in void 0!==r.ref&&(c=r.ref),void 0!==r.key&&(i=""+r.key),r)x.call(r,a)&&!w.hasOwnProperty(a)&&(o[a]=r[a]);var l=arguments.length-2;if(1===l)o.children=n;else if(1<l){for(var s=Array(l),u=0;u<l;u++)s[u]=arguments[u+2];o.children=s}if(e&&e.defaultProps)for(a in l=e.defaultProps)void 0===o[a]&&(o[a]=l[a]);return{$$typeof:t,type:e,key:i,ref:c,props:o,_owner:E.current}}function k(e){return"object"==typeof e&&null!==e&&e.$$typeof===t}var N=/\/+/g;function C(e,t){return"object"==typeof e&&null!==e&&null!=e.key?function(e){var t={"=":"=0",":":"=2"};return"$"+e.replace(/[=:]/g,(function(e){return t[e]}))}(""+e.key):t.toString(36)}function $(e,n,a,o,i){var c=typeof e;"undefined"!==c&&"boolean"!==c||(e=null);var l=!1;if(null===e)l=!0;else switch(c){case"string":case"number":l=!0;break;case"object":switch(e.$$typeof){case t:case r:l=!0}}if(l)return i=i(l=e),e=""===o?"."+C(l,0):o,_(i)?(a="",null!=e&&(a=e.replace(N,"$&/")+"/"),$(i,n,a,"",(function(e){return e}))):null!=i&&(k(i)&&(i=function(e,r){return{$$typeof:t,type:e.type,key:r,ref:e.ref,props:e.props,_owner:e._owner}}(i,a+(!i.key||l&&l.key===i.key?"":(""+i.key).replace(N,"$&/")+"/")+e)),n.push(i)),1;if(l=0,o=""===o?".":o+":",_(e))for(var s=0;s<e.length;s++){var u=o+C(c=e[s],s);l+=$(c,n,a,u,i)}else if(u=function(e){return null===e||"object"!=typeof e?null:"function"==typeof(e=p&&e[p]||e["@@iterator"])?e:null}(e),"function"==typeof u)for(e=u.call(e),s=0;!(c=e.next()).done;)l+=$(c=c.value,n,a,u=o+C(c,s++),i);else if("object"===c)throw n=String(e),Error("Objects are not valid as a React child (found: "+("[object Object]"===n?"object with keys {"+Object.keys(e).join(", ")+"}":n)+"). If you meant to render a collection of children, use an array instead.");return l}function j(e,t,r){if(null==e)return e;var n=[],a=0;return $(e,n,"","",(function(e){return t.call(r,e,a++)})),n}function O(e){if(-1===e._status){var t=e._result;(t=t()).then((function(t){0!==e._status&&-1!==e._status||(e._status=1,e._result=t)}),(function(t){0!==e._status&&-1!==e._status||(e._status=2,e._result=t)})),-1===e._status&&(e._status=0,e._result=t)}if(1===e._status)return e._result.default;throw e._result}var G={current:null},I={transition:null},R={ReactCurrentDispatcher:G,ReactCurrentBatchConfig:I,ReactCurrentOwner:E};e.Children={map:j,forEach:function(e,t,r){j(e,(function(){t.apply(this,arguments)}),r)},count:function(e){var t=0;return j(e,(function(){t++})),t},toArray:function(e){return j(e,(function(e){return e}))||[]},only:function(e){if(!k(e))throw Error("React.Children.only expected to receive a single React element child.");return e}},e.Component=h,e.Fragment=n,e.Profiler=o,e.PureComponent=v,e.StrictMode=a,e.Suspense=s,e.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED=R,e.cloneElement=function(e,r,n){if(null==e)throw Error("React.cloneElement(...): The argument must be a React element, but you passed "+e+".");var a=d({},e.props),o=e.key,i=e.ref,c=e._owner;if(null!=r){if(void 0!==r.ref&&(i=r.ref,c=E.current),void 0!==r.key&&(o=""+r.key),e.type&&e.type.defaultProps)var l=e.type.defaultProps;for(s in r)x.call(r,s)&&!w.hasOwnProperty(s)&&(a[s]=void 0===r[s]&&void 0!==l?l[s]:r[s])}var s=arguments.length-2;if(1===s)a.children=n;else if(1<s){l=Array(s);for(var u=0;u<s;u++)l[u]=arguments[u+2];a.children=l}return{$$typeof:t,type:e.type,key:o,ref:i,props:a,_owner:c}},e.createContext=function(e){return(e={$$typeof:c,_currentValue:e,_currentValue2:e,_threadCount:0,Provider:null,Consumer:null,_defaultValue:null,_globalName:null}).Provider={$$typeof:i,_context:e},e.Consumer=e},e.createElement=S,e.createFactory=function(e){var t=S.bind(null,e);return t.type=e,t},e.createRef=function(){return{current:null}},e.forwardRef=function(e){return{$$typeof:l,render:e}},e.isValidElement=k,e.lazy=function(e){return{$$typeof:f,_payload:{_status:-1,_result:e},_init:O}},e.memo=function(e,t){return{$$typeof:u,type:e,compare:void 0===t?null:t}},e.startTransition=function(e){var t=I.transition;I.transition={};try{e()}finally{I.transition=t}},e.unstable_act=function(){throw Error("act(...) is not supported in production builds of React.")},e.useCallback=function(e,t){return G.current.useCallback(e,t)},e.useContext=function(e){return G.current.useContext(e)},e.useDebugValue=function(){},e.useDeferredValue=function(e){return G.current.useDeferredValue(e)},e.useEffect=function(e,t){return G.current.useEffect(e,t)},e.useId=function(){return G.current.useId()},e.useImperativeHandle=function(e,t,r){return G.current.useImperativeHandle(e,t,r)},e.useInsertionEffect=function(e,t){return G.current.useInsertionEffect(e,t)},e.useLayoutEffect=function(e,t){return G.current.useLayoutEffect(e,t)},e.useMemo=function(e,t){return G.current.useMemo(e,t)},e.useReducer=function(e,t,r){return G.current.useReducer(e,t,r)},e.useRef=function(e){return G.current.useRef(e)},e.useState=function(e){return G.current.useState(e)},e.useSyncExternalStore=function(e,t,r){return G.current.useSyncExternalStore(e,t,r)},e.useTransition=function(){return G.current.useTransition()},e.version="18.2.0"}}),w=(x({"../../node_modules/.pnpm/react@18.2.0/node_modules/react/cjs/react.development.js"(e,t){0}}),x({"../../node_modules/.pnpm/react@18.2.0/node_modules/react/index.js"(e,t){t.exports=E()}})),S=(u=w(),p=null!=u?y(b(u)):{},((e,t,r,n)=>{if(t&&"object"==typeof t||"function"==typeof t)for(let a of v(t))_.call(e,a)||a===r||h(e,a,{get:()=>t[a],enumerable:!(n=g(t,a))||n.enumerable});return e})(!f&&u&&u.__esModule?p:h(p,"default",{value:u,enumerable:!0}),u));function k(e){return S.Children.toArray(e).filter((e=>(0,S.isValidElement)(e)))}(0,a.Gp)((function(e,t){const{ratio:r=4/3,children:o,className:l,...s}=e,u=n.Children.only(o),f=(0,c.cx)("chakra-aspect-ratio",l);return n.createElement(a.m$.div,{ref:t,position:"relative",className:f,_before:{height:0,content:'""',display:"block",paddingBottom:i(r,(e=>1/e*100+"%"))},__css:{"& > *:not(style)":{overflow:"hidden",position:"absolute",top:"0",right:"0",bottom:"0",left:"0",display:"flex",justifyContent:"center",alignItems:"center",width:"100%",height:"100%"},"& > img, & > video":{objectFit:"cover"}},...s},u)})).displayName="AspectRatio",(0,a.Gp)((function(e,t){const r=(0,a.mq)("Badge",e),{className:o,...i}=(0,l.Lr)(e);return n.createElement(a.m$.span,{ref:t,className:(0,c.cx)("chakra-badge",e.className),...i,__css:{display:"inline-block",whiteSpace:"nowrap",verticalAlign:"middle",...r}})})).displayName="Badge";var N=(0,a.m$)("div");N.displayName="Box";var C=(0,a.Gp)((function(e,t){const{size:r,centerContent:a=!0,...o}=e,i=a?{display:"flex",alignItems:"center",justifyContent:"center"}:{};return n.createElement(N,{ref:t,boxSize:r,__css:{...i,flexShrink:0,flexGrow:0},...o})}));C.displayName="Square",(0,a.Gp)((function(e,t){const{size:r,...a}=e;return n.createElement(C,{size:r,ref:t,borderRadius:"9999px",...a})})).displayName="Circle",(0,a.m$)("div",{baseStyle:{display:"flex",alignItems:"center",justifyContent:"center"}}).displayName="Center";var $={horizontal:{insetStart:"50%",transform:"translateX(-50%)"},vertical:{top:"50%",transform:"translateY(-50%)"},both:{insetStart:"50%",top:"50%",transform:"translate(-50%, -50%)"}};(0,a.Gp)((function(e,t){const{axis:r="both",...o}=e;return n.createElement(a.m$.div,{ref:t,__css:$[r],...o,position:"absolute"})}));(0,a.Gp)((function(e,t){const r=(0,a.mq)("Code",e),{className:o,...i}=(0,l.Lr)(e);return n.createElement(a.m$.code,{ref:t,className:(0,c.cx)("chakra-code",e.className),...i,__css:{display:"inline-block",...r}})})).displayName="Code",(0,a.Gp)((function(e,t){const{className:r,centerContent:o,...i}=(0,l.Lr)(e),s=(0,a.mq)("Container",e);return n.createElement(a.m$.div,{ref:t,className:(0,c.cx)("chakra-container",r),...i,__css:{...s,...o&&{display:"flex",flexDirection:"column",alignItems:"center"}}})})).displayName="Container",(0,a.Gp)((function(e,t){const{borderLeftWidth:r,borderBottomWidth:o,borderTopWidth:i,borderRightWidth:s,borderWidth:u,borderStyle:f,borderColor:p,...m}=(0,a.mq)("Divider",e),{className:d,orientation:y="horizontal",__css:h,...g}=(0,l.Lr)(e),v={vertical:{borderLeftWidth:r||s||u||"1px",height:"100%"},horizontal:{borderBottomWidth:o||i||u||"1px",width:"100%"}};return n.createElement(a.m$.hr,{ref:t,"aria-orientation":y,...g,__css:{...m,border:"0",borderColor:p,borderStyle:f,...v[y],...h},className:(0,c.cx)("chakra-divider",d)})})).displayName="Divider";var j=(0,a.Gp)((function(e,t){const{direction:r,align:o,justify:i,wrap:c,basis:l,grow:s,shrink:u,...f}=e,p={display:"flex",flexDirection:r,alignItems:o,justifyContent:i,flexWrap:c,flexBasis:l,flexGrow:s,flexShrink:u};return n.createElement(a.m$.div,{ref:t,__css:p,...f})}));j.displayName="Flex";var O=(0,a.Gp)((function(e,t){const{templateAreas:r,gap:o,rowGap:i,columnGap:c,column:l,row:s,autoFlow:u,autoRows:f,templateRows:p,autoColumns:m,templateColumns:d,...y}=e,h={display:"grid",gridTemplateAreas:r,gridGap:o,gridRowGap:i,gridColumnGap:c,gridAutoColumns:m,gridColumn:l,gridRow:s,gridAutoFlow:u,gridAutoRows:f,gridTemplateRows:p,gridTemplateColumns:d};return n.createElement(a.m$.div,{ref:t,__css:h,...y})}));function G(e){return i(e,(e=>"auto"===e?"auto":`span ${e}/span ${e}`))}O.displayName="Grid",(0,a.Gp)((function(e,t){const{area:r,colSpan:o,colStart:i,colEnd:c,rowEnd:l,rowSpan:u,rowStart:f,...p}=e,m=s({gridArea:r,gridColumn:G(o),gridRow:G(u),gridColumnStart:i,gridColumnEnd:c,gridRowStart:f,gridRowEnd:l});return n.createElement(a.m$.div,{ref:t,__css:m,...p})})).displayName="GridItem";var I=(0,a.Gp)((function(e,t){const r=(0,a.mq)("Heading",e),{className:o,...i}=(0,l.Lr)(e);return n.createElement(a.m$.h2,{ref:t,className:(0,c.cx)("chakra-heading",e.className),...i,__css:r})}));I.displayName="Heading";(0,a.Gp)((function(e,t){const r=(0,a.mq)("Mark",e),o=(0,l.Lr)(e);return n.createElement(N,{ref:t,...o,as:"mark",__css:{bg:"transparent",whiteSpace:"nowrap",...r}})}));(0,a.Gp)((function(e,t){const r=(0,a.mq)("Kbd",e),{className:o,...i}=(0,l.Lr)(e);return n.createElement(a.m$.kbd,{ref:t,className:(0,c.cx)("chakra-kbd",o),...i,__css:{fontFamily:"mono",...r}})})).displayName="Kbd";var R=(0,a.Gp)((function(e,t){const r=(0,a.mq)("Link",e),{className:o,isExternal:i,...s}=(0,l.Lr)(e);return n.createElement(a.m$.a,{target:i?"_blank":void 0,rel:i?"noopener":void 0,ref:t,className:(0,c.cx)("chakra-link",o),...s,__css:r})}));R.displayName="Link";(0,a.Gp)((function(e,t){const{isExternal:r,target:o,rel:i,className:l,...s}=e;return n.createElement(a.m$.a,{...s,ref:t,className:(0,c.cx)("chakra-linkbox__overlay",l),rel:r?"noopener noreferrer":i,target:r?"_blank":o,__css:{position:"static","&::before":{content:"''",cursor:"inherit",display:"block",position:"absolute",top:0,left:0,zIndex:0,width:"100%",height:"100%"}}})})),(0,a.Gp)((function(e,t){const{className:r,...o}=e;return n.createElement(a.m$.div,{ref:t,position:"relative",...o,className:(0,c.cx)("chakra-linkbox",r),__css:{"a[href]:not(.chakra-linkbox__overlay), abbr[title]":{position:"relative",zIndex:1}}})}));var[P,L]=(0,d.k)({name:"ListStylesContext",errorMessage:"useListStyles returned is 'undefined'. Seems you forgot to wrap the components in \"<List />\" "}),A=(0,a.Gp)((function(e,t){const r=(0,a.jC)("List",e),{children:o,styleType:i="none",stylePosition:c,spacing:s,...u}=(0,l.Lr)(e),f=k(o),p=s?{"& > *:not(style) ~ *:not(style)":{mt:s}}:{};return n.createElement(P,{value:r},n.createElement(a.m$.ul,{ref:t,listStyleType:i,listStylePosition:c,role:"list",__css:{...r.container,...p},...u},f))}));A.displayName="List",(0,a.Gp)(((e,t)=>{const{as:r,...a}=e;return n.createElement(A,{ref:t,as:"ol",styleType:"decimal",marginStart:"1em",...a})})).displayName="OrderedList",(0,a.Gp)((function(e,t){const{as:r,...a}=e;return n.createElement(A,{ref:t,as:"ul",styleType:"initial",marginStart:"1em",...a})})).displayName="UnorderedList",(0,a.Gp)((function(e,t){const r=L();return n.createElement(a.m$.li,{ref:t,...e,__css:r.item})})).displayName="ListItem",(0,a.Gp)((function(e,t){const r=L();return n.createElement(m.JO,{ref:t,role:"presentation",...e,__css:r.icon})})).displayName="ListIcon",(0,a.Gp)((function(e,t){const{columns:r,spacingX:o,spacingY:c,spacing:l,minChildWidth:s,...u}=e,f=(0,a.Fg)(),p=s?function(e,t){return i(e,(e=>{const r=(0,a.LP)("sizes",e,"number"==typeof(n=e)?`${n}px`:n)(t);var n;return null===e?null:`repeat(auto-fit, minmax(${r}, 1fr))`}))}(s,f):i(r,(e=>null===e?null:`repeat(${e}, minmax(0, 1fr))`));return n.createElement(O,{ref:t,gap:l,columnGap:o,rowGap:c,templateColumns:p,...u})})).displayName="SimpleGrid",(0,a.m$)("div",{baseStyle:{flex:1,justifySelf:"stretch",alignSelf:"stretch"}}).displayName="Spacer";var T="& > *:not(style) ~ *:not(style)";var B=e=>n.createElement(a.m$.div,{className:"chakra-stack__item",...e,__css:{display:"inline-block",flex:"0 0 auto",minWidth:0,...e.__css}});B.displayName="StackItem";var W=(0,a.Gp)(((e,t)=>{const{isInline:r,direction:o,align:l,justify:s,spacing:u="0.5rem",wrap:f,children:p,divider:m,className:d,shouldWrapChildren:y,...h}=e,g=r?"row":o??"column",v=(0,n.useMemo)((()=>function(e){const{spacing:t,direction:r}=e,n={column:{marginTop:t,marginEnd:0,marginBottom:0,marginStart:0},row:{marginTop:0,marginEnd:0,marginBottom:0,marginStart:t},"column-reverse":{marginTop:0,marginEnd:0,marginBottom:t,marginStart:0},"row-reverse":{marginTop:0,marginEnd:t,marginBottom:0,marginStart:0}};return{flexDirection:r,[T]:i(r,(e=>n[e]))}}({direction:g,spacing:u})),[g,u]),b=(0,n.useMemo)((()=>function(e){const{spacing:t,direction:r}=e,n={column:{my:t,mx:0,borderLeftWidth:0,borderBottomWidth:"1px"},"column-reverse":{my:t,mx:0,borderLeftWidth:0,borderBottomWidth:"1px"},row:{mx:t,my:0,borderLeftWidth:"1px",borderBottomWidth:0},"row-reverse":{mx:t,my:0,borderLeftWidth:"1px",borderBottomWidth:0}};return{"&":i(r,(e=>n[e]))}}({spacing:u,direction:g})),[u,g]),_=!!m,x=!y&&!_,E=k(p),w=x?E:E.map(((e,t)=>{const r=void 0!==e.key?e.key:t,a=t+1===E.length,o=y?n.createElement(B,{key:r},e):e;if(!_)return o;const i=(0,n.cloneElement)(m,{__css:b}),c=a?null:i;return n.createElement(n.Fragment,{key:r},o,c)})),S=(0,c.cx)("chakra-stack",d);return n.createElement(a.m$.div,{ref:t,display:"flex",alignItems:l,justifyContent:s,flexDirection:v.flexDirection,flexWrap:f,className:S,__css:_?{}:{[T]:v[T]},...h},w)}));W.displayName="Stack";var z=(0,a.Gp)(((e,t)=>n.createElement(W,{align:"center",...e,direction:"row",ref:t})));z.displayName="HStack";var q=(0,a.Gp)(((e,t)=>n.createElement(W,{align:"center",...e,direction:"column",ref:t})));q.displayName="VStack";var D=(0,a.Gp)((function(e,t){const r=(0,a.mq)("Text",e),{className:o,align:i,decoration:u,casing:f,...p}=(0,l.Lr)(e),m=s({textAlign:e.align,textDecoration:e.decoration,textTransform:e.casing});return n.createElement(a.m$.p,{ref:t,className:(0,c.cx)("chakra-text",e.className),...m,...p,__css:r})}));function M(e){return"number"==typeof e?`${e}px`:e}D.displayName="Text",(0,a.Gp)((function(e,t){const{spacing:r="0.5rem",spacingX:o,spacingY:s,children:u,justify:f,direction:p,align:m,className:d,shouldWrapChildren:y,...h}=e,g=(0,n.useMemo)((()=>{const{spacingX:e=r,spacingY:t=r}={spacingX:o,spacingY:s};return{"--chakra-wrap-x-spacing":t=>i(e,(e=>M((0,l.fr)("space",e)(t)))),"--chakra-wrap-y-spacing":e=>i(t,(t=>M((0,l.fr)("space",t)(e)))),"--wrap-x-spacing":"calc(var(--chakra-wrap-x-spacing) / 2)","--wrap-y-spacing":"calc(var(--chakra-wrap-y-spacing) / 2)",display:"flex",flexWrap:"wrap",justifyContent:f,alignItems:m,flexDirection:p,listStyleType:"none",padding:"0",margin:"calc(var(--wrap-y-spacing) * -1) calc(var(--wrap-x-spacing) * -1)","& > *:not(style)":{margin:"var(--wrap-y-spacing) var(--wrap-x-spacing)"}}}),[r,o,s,f,m,p]),v=y?n.Children.map(u,((e,t)=>n.createElement(U,{key:t},e))):u;return n.createElement(a.m$.div,{ref:t,className:(0,c.cx)("chakra-wrap",d),overflow:"hidden",...h},n.createElement(a.m$.ul,{className:"chakra-wrap__list",__css:g},v))})).displayName="Wrap";var U=(0,a.Gp)((function(e,t){const{className:r,...o}=e;return n.createElement(a.m$.li,{ref:t,__css:{display:"flex",alignItems:"flex-start"},className:(0,c.cx)("chakra-wrap__listitem",r),...o})}));U.displayName="WrapItem"},8089:(e,t,r)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.default=void 0;var n=r(79),a=r(7126),o=r(5250);function i(e){return i="function"==typeof Symbol&&"symbol"==typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&"function"==typeof Symbol&&e.constructor===Symbol&&e!==Symbol.prototype?"symbol":typeof e},i(e)}var c=function(e){var t=function(e){if(!["undefined","string"].includes(i(e.idPrefix)))throw new Error('Invalid value passed for prop "idPrefix", expected undefined or string, got '.concat(i(e.idPrefix)));return e}(e),r=t.children,c=t.idPrefix,l=void 0===c?"i":c,s=(0,n.useRef)(0),u=(0,n.useCallback)((function(){return"".concat(l).concat(s.current++)}),[]);return(0,o.jsx)(a.UniqueIdGeneratorContextProvider,{value:u,children:r})};t.default=c},7126:(e,t,r)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.UniqueIdGeneratorContextProvider=t.useUniqueIdGenerator=void 0;var n,a=r(79),o=(n=0,function(){return"i".concat(n++)}),i=(0,a.createContext)(o);t.useUniqueIdGenerator=function(){return(0,a.useContext)(i)};var c=i.Provider;t.UniqueIdGeneratorContextProvider=c},7331:(e,t,r)=>{Object.defineProperty(t,"uB",{enumerable:!0,get:function(){return n.default}});var n=o(r(6873)),a=o(r(8089));function o(e){return e&&e.__esModule?e:{default:e}}},6873:(e,t,r)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.default=void 0;var n=r(79),a=r(7126);function o(e,t){return function(e){if(Array.isArray(e))return e}(e)||function(e,t){var r=null==e?null:"undefined"!=typeof Symbol&&e[Symbol.iterator]||e["@@iterator"];if(null==r)return;var n,a,o=[],i=!0,c=!1;try{for(r=r.call(e);!(i=(n=r.next()).done)&&(o.push(n.value),!t||o.length!==t);i=!0);}catch(l){c=!0,a=l}finally{try{i||null==r.return||r.return()}finally{if(c)throw a}}return o}(e,t)||function(e,t){if(!e)return;if("string"==typeof e)return i(e,t);var r=Object.prototype.toString.call(e).slice(8,-1);"Object"===r&&e.constructor&&(r=e.constructor.name);if("Map"===r||"Set"===r)return Array.from(e);if("Arguments"===r||/^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(r))return i(e,t)}(e,t)||function(){throw new TypeError("Invalid attempt to destructure non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")}()}function i(e,t){(null==t||t>e.length)&&(t=e.length);for(var r=0,n=new Array(t);r<t;r++)n[r]=e[r];return n}var c=function(){var e=(0,a.useUniqueIdGenerator)();return o((0,n.useState)(e),1)[0]};t.default=c},4520:(e,t,r)=>{r.d(t,{Mp$:()=>f});var n=r(79),a={color:void 0,size:void 0,className:void 0,style:void 0,attr:void 0},o=n.createContext&&n.createContext(a),i=function(){return i=Object.assign||function(e){for(var t,r=1,n=arguments.length;r<n;r++)for(var a in t=arguments[r])Object.prototype.hasOwnProperty.call(t,a)&&(e[a]=t[a]);return e},i.apply(this,arguments)},c=function(e,t){var r={};for(var n in e)Object.prototype.hasOwnProperty.call(e,n)&&t.indexOf(n)<0&&(r[n]=e[n]);if(null!=e&&"function"==typeof Object.getOwnPropertySymbols){var a=0;for(n=Object.getOwnPropertySymbols(e);a<n.length;a++)t.indexOf(n[a])<0&&Object.prototype.propertyIsEnumerable.call(e,n[a])&&(r[n[a]]=e[n[a]])}return r};function l(e){return e&&e.map((function(e,t){return n.createElement(e.tag,i({key:t},e.attr),l(e.child))}))}function s(e){return function(t){return n.createElement(u,i({attr:i({},e.attr)},t),l(e.child))}}function u(e){var t=function(t){var r,a=e.attr,o=e.size,l=e.title,s=c(e,["attr","size","title"]),u=o||t.size||"1em";return t.className&&(r=t.className),e.className&&(r=(r?r+" ":"")+e.className),n.createElement("svg",i({stroke:"currentColor",fill:"currentColor",strokeWidth:"0"},t.attr,a,s,{className:r,style:i(i({color:e.color||t.color},t.style),e.style),height:u,width:u,xmlns:"http://www.w3.org/2000/svg"}),l&&n.createElement("title",null,l),e.children)};return void 0!==o?n.createElement(o.Consumer,null,(function(e){return t(e)})):t(a)}function f(e){return s({tag:"svg",attr:{viewBox:"0 0 448 512"},child:[{tag:"path",attr:{d:"M448 360V24c0-13.3-10.7-24-24-24H96C43 0 0 43 0 96v320c0 53 43 96 96 96h328c13.3 0 24-10.7 24-24v-16c0-7.5-3.5-14.3-8.9-18.7-4.2-15.4-4.2-59.3 0-74.7 5.4-4.3 8.9-11.1 8.9-18.6zM128 134c0-3.3 2.7-6 6-6h212c3.3 0 6 2.7 6 6v20c0 3.3-2.7 6-6 6H134c-3.3 0-6-2.7-6-6v-20zm0 64c0-3.3 2.7-6 6-6h212c3.3 0 6 2.7 6 6v20c0 3.3-2.7 6-6 6H134c-3.3 0-6-2.7-6-6v-20zm253.4 250H96c-17.7 0-32-14.3-32-32 0-17.6 14.4-32 32-32h285.4c-1.9 17.1-1.9 46.9 0 64z"}}]})(e)}},2875:(e,t,r)=>{var n=r(79),a=Symbol.for("react.element"),o=Symbol.for("react.fragment"),i=Object.prototype.hasOwnProperty,c=n.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED.ReactCurrentOwner,l={key:!0,ref:!0,__self:!0,__source:!0};function s(e,t,r){var n,o={},s=null,u=null;for(n in void 0!==r&&(s=""+r),void 0!==t.key&&(s=""+t.key),void 0!==t.ref&&(u=t.ref),t)i.call(t,n)&&!l.hasOwnProperty(n)&&(o[n]=t[n]);if(e&&e.defaultProps)for(n in t=e.defaultProps)void 0===o[n]&&(o[n]=t[n]);return{$$typeof:a,type:e,key:s,ref:u,props:o,_owner:c.current}}t.Fragment=o,t.jsx=s,t.jsxs=s},5250:(e,t,r)=>{e.exports=r(2875)}}]);