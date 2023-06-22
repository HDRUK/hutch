import React from "react";
import HdrLogo from "@site/static/img/hdruk_logo.svg";
import TreFxLogo from "@site/static/img/tre-fx_logo.svg";
import DareLogo from "@site/static/img/DARE-UK_logo.svg";
import UkriLogo from "@site/static/img/UKRI_logo.svg";
import { HStack, Link } from "@chakra-ui/react";

export const Funders = () => {
  return (
    <HStack spacing={4} justify={"center"} bgColor={"gray.50"} my={4}>
      <Link href="https://www.hdruk.ac.uk/" isExternal title="HDR UK">
        <HdrLogo role="img" />
      </Link>
      <Link href="https://trefx.uk/" isExternal title="UKRI">
        <UkriLogo role="img" />
      </Link>
      <Link href="https://dareuk.org.uk/" isExternal title="DARE UK">
        <DareLogo role="img" />
      </Link>
      <Link href="https://www.hdruk.ac.uk/" isExternal title="TRE-FX">
        <TreFxLogo role="img" />
      </Link>
    </HStack>
  )
};
