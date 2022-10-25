import { Box } from "@chakra-ui/react";
import React from "react";
import { ReactComponent as ColorLogo } from "/public/assets/Hutch-color.svg";
import { ReactComponent as MonoLogo } from "/public/assets/Hutch-mono.svg";

export const HutchLogo = ({ logoColor, logoMaxWidth, logoFillColor }) => {
  const logoSize = logoMaxWidth ? logoMaxWidth : "50px"; // default logo size/width;
  const logoColorFill = logoFillColor ? logoFillColor : "#fff"; // default logo color

  const logo = logoColor ? (
    <ColorLogo style={{ color: logoColorFill }} />
  ) : (
    <MonoLogo style={{ color: logoColorFill }} />
  );

  return (
    <Box maxW={logoSize} p={"5px 2px"}>
      {logo}
    </Box>
  );
};
