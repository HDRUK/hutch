import React from "react";
export const HutchLogo = ({ logoColor, logoMaxWidth, logoFillColor }) => {
  const logoSize = logoMaxWidth ? logoMaxWidth : "50px"; // default logo size/width;
  const logoColorFill = logoFillColor ? logoFillColor : "#fff"; // default logo color

  const coloredLogo = () => {
    const colorLogoFill = logoColorFill;
    return (
      <svg
        width="100%"
        height="100%"
        viewBox="0 0 612 228"
        version="1.1"
        xmlns="http://www.w3.org/2000/svg"
        xmlnsXlink="http://www.w3.org/1999/xlink"
        xmlSpace="preserve"
        xmlnsserif="http://www.serif.com/"
        style={{
          fillRule: "evenodd",
          clipRule: "evenodd",
          strokeLinejoin: "round",
          strokeMiterlimit: "2",
          maxWidth: logoSize,
          padding: "5px 2px",
          marginBottom: "10px",
        }}
      >
        <g id="Hutch-color-white" serifid="Hutch color white">
          <g>
            <g id="Rabbit-mono" serifid="Rabbit mono">
              <path
                id="Ear"
                d="M352.33,42.277c33.695,2.337 57.147,24.062 56.539,26.582c-0.679,2.812 -26.865,-7.155 -60.639,-7.485c-32.651,-0.319 -55.943,25.118 -59.106,6.056c-1.946,-11.729 27.015,-27.664 63.206,-25.153Z"
                style={{ fill: colorLogoFill }}
              />
              <path
                id="Ear1"
                serifid="Ear"
                d="M377.773,10.822c28.312,18.419 42.769,45.414 41.012,47.321c-1.959,2.127 -35.06,-23.629 -56.693,-32.245c-33.477,-13.335 -69.204,7.567 -62.704,-10.63c3.999,-11.196 47.975,-24.228 78.385,-4.446Z"
                style={{ fill: colorLogoFill }}
              />
              <path
                id="Head"
                d="M570.123,207.273c21.883,-19 35.633,-46.417 35.633,-76.881c0,-57.306 -48.658,-103.832 -108.59,-103.832c-39.161,0 -73.507,19.864 -92.608,49.615c17.65,-36.398 54.964,-61.518 98.098,-61.518c60.135,0 108.957,48.822 108.957,108.957c-0,33.6 -15.242,63.668 -39.18,83.659l-2.31,0Z"
                style={{ fill: colorLogoFill }}
              />
            </g>
            <use
              id="Eye"
              xlinkHref="#_Image1"
              x="451.393"
              y="42.191"
              width="40px"
              height="40px"
            />
          </g>
          <g transform="matrix(1,0,0,1,-510.35,-94.6236)">
            <g transform="matrix(200,0,0,200,510.511,286.979)">
              <path
                d="M0.026,-0c-0.007,-0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.011 -0.004,-0.019l0.139,-0.648c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.048,0.226l0.204,0l0.048,-0.226c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.007,-0.005 0.014,-0.008 0.021,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.138,0.648c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.162,0c-0.007,0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.012 -0.004,-0.019l0.05,-0.232l-0.204,0l-0.05,0.232c-0.001,0.007 -0.005,0.013 -0.011,0.018c-0.006,0.005 -0.013,0.008 -0.02,0.008l-0.162,-0Z"
                style={{ fill: colorLogoFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,662.111,286.979)">
              <path
                d="M0.319,0.01c-0.043,-0 -0.083,-0.006 -0.119,-0.017c-0.038,-0.011 -0.069,-0.027 -0.096,-0.05c-0.026,-0.022 -0.044,-0.05 -0.055,-0.085c-0.01,-0.035 -0.01,-0.076 -0,-0.123l0.087,-0.409c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.005,0.005 0.006,0.011 0.005,0.018l-0.087,0.406c-0.007,0.034 -0.005,0.06 0.007,0.077c0.011,0.017 0.033,0.026 0.066,0.026c0.032,0 0.058,-0.009 0.078,-0.026c0.021,-0.017 0.035,-0.043 0.042,-0.077l0.087,-0.406c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.163,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.087,0.409c-0.02,0.095 -0.058,0.164 -0.115,0.209c-0.057,0.044 -0.136,0.066 -0.238,0.066Z"
                style={{ fill: colorLogoFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,773.296,286.979)">
              <path
                d="M0.179,-0c-0.007,-0 -0.012,-0.003 -0.016,-0.008c-0.005,-0.005 -0.006,-0.011 -0.005,-0.018l0.105,-0.488l-0.172,0c-0.007,0 -0.013,-0.002 -0.017,-0.007c-0.005,-0.005 -0.006,-0.012 -0.005,-0.019l0.029,-0.134c0.001,-0.007 0.005,-0.013 0.012,-0.018c0.006,-0.005 0.012,-0.008 0.019,-0.008l0.562,0c0.007,0 0.013,0.003 0.018,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.029,0.134c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.172,0l-0.105,0.488c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.166,0Z"
                style={{ fill: colorLogoFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,871.472,286.979)">
              <path
                d="M0.317,0.01c-0.045,-0 -0.087,-0.006 -0.125,-0.017c-0.038,-0.011 -0.07,-0.027 -0.096,-0.05c-0.026,-0.022 -0.045,-0.05 -0.056,-0.084c-0.011,-0.034 -0.012,-0.074 -0.005,-0.12c0.005,-0.027 0.01,-0.057 0.017,-0.088c0.007,-0.031 0.014,-0.061 0.021,-0.09c0.016,-0.06 0.04,-0.11 0.072,-0.151c0.032,-0.04 0.072,-0.07 0.121,-0.09c0.048,-0.02 0.104,-0.03 0.169,-0.03c0.041,0 0.081,0.005 0.119,0.015c0.038,0.011 0.071,0.026 0.099,0.047c0.029,0.021 0.05,0.046 0.063,0.077c0.013,0.031 0.016,0.068 0.008,0.109c-0.001,0.006 -0.004,0.011 -0.009,0.015c-0.005,0.005 -0.011,0.007 -0.017,0.007l-0.17,-0c-0.009,-0 -0.016,-0.002 -0.02,-0.006c-0.005,-0.004 -0.008,-0.011 -0.009,-0.021c-0.001,-0.028 -0.008,-0.047 -0.023,-0.057c-0.015,-0.011 -0.035,-0.016 -0.059,-0.016c-0.029,0 -0.054,0.008 -0.075,0.024c-0.021,0.015 -0.037,0.043 -0.046,0.082c-0.014,0.055 -0.026,0.111 -0.036,0.168c-0.007,0.039 -0.004,0.067 0.01,0.083c0.013,0.015 0.035,0.023 0.064,0.023c0.024,0 0.046,-0.005 0.066,-0.016c0.019,-0.011 0.036,-0.03 0.049,-0.057c0.005,-0.011 0.011,-0.018 0.017,-0.022c0.005,-0.003 0.013,-0.005 0.023,-0.005l0.17,0c0.006,0 0.011,0.002 0.015,0.006c0.003,0.004 0.004,0.009 0.003,0.015c-0.011,0.041 -0.026,0.077 -0.047,0.108c-0.021,0.032 -0.048,0.058 -0.079,0.079c-0.031,0.021 -0.067,0.036 -0.106,0.047c-0.039,0.01 -0.082,0.015 -0.128,0.015Z"
                style={{ fill: colorLogoFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,979.783,286.979)">
              <path
                d="M0.026,-0c-0.007,-0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.011 -0.004,-0.019l0.139,-0.648c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.048,0.226l0.204,0l0.048,-0.226c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.007,-0.005 0.014,-0.008 0.021,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.138,0.648c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.162,0c-0.007,0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.012 -0.004,-0.019l0.05,-0.232l-0.204,0l-0.05,0.232c-0.001,0.007 -0.005,0.013 -0.011,0.018c-0.006,0.005 -0.013,0.008 -0.02,0.008l-0.162,-0Z"
                style={{ fill: colorLogoFill, fillRule: "nonzero" }}
              />
            </g>
          </g>
          <g id="Hutch">
            <path
              d="M297.056,52.038c-8.474,6.022 -11.744,10.98 -9.908,17.951c1.26,4.787 5.008,6.556 8.53,7.755l-116.348,-0.449c-0.768,-0 -1.496,-0.657 -1.988,-1.793c-0.492,-1.136 -0.698,-2.635 -0.562,-4.093c0.429,-4.599 2.612,-11.008 2.991,-15.078c0.222,-2.379 2.492,-3.972 3.745,-3.976l113.54,-0.317Z"
              style={{ fill: "#d4a67f" }}
            />
            <path
              d="M586.896,207.273c-0,-2.761 -2.239,-5 -5,-5c-53.668,0 -522.361,0 -576.03,0c-2.761,0 -5,2.239 -5,5c0,4.431 0,10.867 0,15.298c0,2.761 2.239,5 5,5c53.669,-0 522.362,-0 576.03,-0c2.761,-0 5,-2.239 5,-5c-0,-4.431 -0,-10.867 -0,-15.298Z"
              style={{ fill: "#d4a67f" }}
            />
          </g>
        </g>
        <defs>
          <image
            id="_Image1"
            width="40px"
            height="40px"
            xlinkHref="data:image/png;base64,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"
          />
        </defs>
      </svg>
    );
  };

  const defaultLogo = () => {
    return (
      <svg
        width="100%"
        height="100%"
        viewBox="0 0 612 228"
        version="1.1"
        xmlns="http://www.w3.org/2000/svg"
        xmlnsXlink="http://www.w3.org/1999/xlink"
        xmlSpace="preserve"
        xmlnsserif="http://www.serif.com/"
        style={{
          fillRule: "evenodd",
          clipRule: "evenodd",
          strokeLinejoin: "round",
          strokeMiterlimit: "2",
          maxWidth: logoSize,
          padding: "5px 2px",
          marginBottom: "10px",
        }}
      >
        <g id="Hutch-mono-white" serifid="Hutch mono white">
          <g id="Rabbit">
            <path
              id="Ear"
              d="M352.077,42.277c33.694,2.337 57.146,24.062 56.538,26.582c-0.678,2.812 -26.865,-7.155 -60.639,-7.485c-32.65,-0.319 -55.943,25.118 -59.106,6.056c-1.946,-11.729 27.015,-27.664 63.207,-25.153Z"
              style={{ fill: logoColorFill }}
            />
            <path
              id="Ear1"
              serifid="Ear"
              d="M377.519,10.822c28.312,18.419 42.769,45.414 41.013,47.321c-1.96,2.127 -35.06,-23.629 -56.694,-32.245c-33.477,-13.335 -69.204,7.567 -62.704,-10.63c3.999,-11.196 47.975,-24.228 78.385,-4.446Z"
              style={{ fill: logoColorFill }}
            />
            <circle
              id="Eye"
              cx="471.14"
              cy="62.191"
              r="20"
              style={{ fill: logoColorFill }}
            />
            <path
              id="Head"
              d="M569.87,207.273c21.882,-19 35.632,-46.417 35.632,-76.881c0,-57.306 -48.657,-103.832 -108.59,-103.832c-39.16,0 -73.507,19.864 -92.608,49.615c17.65,-36.398 54.964,-61.518 98.099,-61.518c60.134,0 108.956,48.822 108.956,108.957c0,33.6 -15.242,63.668 -39.18,83.659l-2.309,0Z"
              style={{ fill: logoColorFill }}
            />
          </g>
          <g transform="matrix(1,0,0,1,-510.604,-94.6236)">
            <g transform="matrix(200,0,0,200,510.511,286.979)">
              <path
                d="M0.026,-0c-0.007,-0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.011 -0.004,-0.019l0.139,-0.648c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.048,0.226l0.204,0l0.048,-0.226c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.007,-0.005 0.014,-0.008 0.021,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.138,0.648c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.162,0c-0.007,0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.012 -0.004,-0.019l0.05,-0.232l-0.204,0l-0.05,0.232c-0.001,0.007 -0.005,0.013 -0.011,0.018c-0.006,0.005 -0.013,0.008 -0.02,0.008l-0.162,-0Z"
                style={{ fill: logoColorFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,662.111,286.979)">
              <path
                d="M0.319,0.01c-0.043,-0 -0.083,-0.006 -0.119,-0.017c-0.038,-0.011 -0.069,-0.027 -0.096,-0.05c-0.026,-0.022 -0.044,-0.05 -0.055,-0.085c-0.01,-0.035 -0.01,-0.076 -0,-0.123l0.087,-0.409c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.005,0.005 0.006,0.011 0.005,0.018l-0.087,0.406c-0.007,0.034 -0.005,0.06 0.007,0.077c0.011,0.017 0.033,0.026 0.066,0.026c0.032,0 0.058,-0.009 0.078,-0.026c0.021,-0.017 0.035,-0.043 0.042,-0.077l0.087,-0.406c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.163,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.087,0.409c-0.02,0.095 -0.058,0.164 -0.115,0.209c-0.057,0.044 -0.136,0.066 -0.238,0.066Z"
                style={{ fill: logoColorFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,773.296,286.979)">
              <path
                d="M0.179,-0c-0.007,-0 -0.012,-0.003 -0.016,-0.008c-0.005,-0.005 -0.006,-0.011 -0.005,-0.018l0.105,-0.488l-0.172,0c-0.007,0 -0.013,-0.002 -0.017,-0.007c-0.005,-0.005 -0.006,-0.012 -0.005,-0.019l0.029,-0.134c0.001,-0.007 0.005,-0.013 0.012,-0.018c0.006,-0.005 0.012,-0.008 0.019,-0.008l0.562,0c0.007,0 0.013,0.003 0.018,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.029,0.134c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.172,0l-0.105,0.488c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.166,0Z"
                style={{ fill: logoColorFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,871.472,286.979)">
              <path
                d="M0.317,0.01c-0.045,-0 -0.087,-0.006 -0.125,-0.017c-0.038,-0.011 -0.07,-0.027 -0.096,-0.05c-0.026,-0.022 -0.045,-0.05 -0.056,-0.084c-0.011,-0.034 -0.012,-0.074 -0.005,-0.12c0.005,-0.027 0.01,-0.057 0.017,-0.088c0.007,-0.031 0.014,-0.061 0.021,-0.09c0.016,-0.06 0.04,-0.11 0.072,-0.151c0.032,-0.04 0.072,-0.07 0.121,-0.09c0.048,-0.02 0.104,-0.03 0.169,-0.03c0.041,0 0.081,0.005 0.119,0.015c0.038,0.011 0.071,0.026 0.099,0.047c0.029,0.021 0.05,0.046 0.063,0.077c0.013,0.031 0.016,0.068 0.008,0.109c-0.001,0.006 -0.004,0.011 -0.009,0.015c-0.005,0.005 -0.011,0.007 -0.017,0.007l-0.17,-0c-0.009,-0 -0.016,-0.002 -0.02,-0.006c-0.005,-0.004 -0.008,-0.011 -0.009,-0.021c-0.001,-0.028 -0.008,-0.047 -0.023,-0.057c-0.015,-0.011 -0.035,-0.016 -0.059,-0.016c-0.029,0 -0.054,0.008 -0.075,0.024c-0.021,0.015 -0.037,0.043 -0.046,0.082c-0.014,0.055 -0.026,0.111 -0.036,0.168c-0.007,0.039 -0.004,0.067 0.01,0.083c0.013,0.015 0.035,0.023 0.064,0.023c0.024,0 0.046,-0.005 0.066,-0.016c0.019,-0.011 0.036,-0.03 0.049,-0.057c0.005,-0.011 0.011,-0.018 0.017,-0.022c0.005,-0.003 0.013,-0.005 0.023,-0.005l0.17,0c0.006,0 0.011,0.002 0.015,0.006c0.003,0.004 0.004,0.009 0.003,0.015c-0.011,0.041 -0.026,0.077 -0.047,0.108c-0.021,0.032 -0.048,0.058 -0.079,0.079c-0.031,0.021 -0.067,0.036 -0.106,0.047c-0.039,0.01 -0.082,0.015 -0.128,0.015Z"
                style={{ fill: logoColorFill, fillRule: "nonzero" }}
              />
            </g>
            <g transform="matrix(151.273,0,0,151.273,979.783,286.979)">
              <path
                d="M0.026,-0c-0.007,-0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.011 -0.004,-0.019l0.139,-0.648c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.006,-0.005 0.013,-0.008 0.02,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.048,0.226l0.204,0l0.048,-0.226c0.001,-0.007 0.005,-0.013 0.011,-0.018c0.007,-0.005 0.014,-0.008 0.021,-0.008l0.162,0c0.007,0 0.013,0.003 0.017,0.008c0.004,0.005 0.005,0.011 0.004,0.018l-0.138,0.648c-0.001,0.007 -0.005,0.014 -0.011,0.019c-0.006,0.005 -0.013,0.007 -0.02,0.007l-0.162,0c-0.007,0 -0.013,-0.002 -0.018,-0.007c-0.004,-0.005 -0.005,-0.012 -0.004,-0.019l0.05,-0.232l-0.204,0l-0.05,0.232c-0.001,0.007 -0.005,0.013 -0.011,0.018c-0.006,0.005 -0.013,0.008 -0.02,0.008l-0.162,-0Z"
                style={{ fill: logoColorFill, fillRule: "nonzero" }}
              />
            </g>
          </g>
          <g id="Hutch">
            <path
              d="M296.802,52.038c-8.474,6.022 -11.743,10.98 -9.908,17.951c1.261,4.787 5.008,6.556 8.531,7.755l-116.349,-0.449c-0.767,-0 -1.496,-0.657 -1.988,-1.793c-0.492,-1.136 -0.698,-2.635 -0.562,-4.093c0.429,-4.599 2.612,-11.008 2.992,-15.078c0.222,-2.379 2.492,-3.972 3.744,-3.976l113.54,-0.317Z"
              style={{ fill: logoColorFill }}
            />
            <path
              d="M586.642,207.273c0,-2.761 -2.238,-5 -5,-5c-53.668,0 -522.361,0 -576.029,0c-2.762,0 -5,2.239 -5,5c-0,4.431 -0,10.867 -0,15.298c-0,2.761 2.238,5 5,5c53.668,-0 522.361,-0 576.029,-0c2.762,-0 5,-2.239 5,-5c0,-4.431 0,-10.867 0,-15.298Z"
              style={{ fill: logoColorFill }}
            />
          </g>
        </g>
      </svg>
    );
  };

  const logo = logoColor ? coloredLogo() : defaultLogo(); //logo output

  return logo;
};
