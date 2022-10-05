import React from "react";
import DocsLink from "@docusaurus/Link";
import { Heading, Link, useColorModeValue, VStack } from "@chakra-ui/react";

export const LinkCard = ({ heading, children, href }) => {
  const bgColors = {
    light: {
      normal: "gray.200",
      hover: "gray.100",
    },
    dark: { normal: "gray.700", hover: "gray.600" },
  };
  const bg = useColorModeValue(bgColors.light, bgColors.dark);
  return (
    <Link
      as={DocsLink}
      to={href}
      color="inherit"
      _hover={{ textDecoration: "inherit", color: "inherit" }}
    >
      <VStack
        h="100%"
        bg={bg.normal}
        p={4}
        borderRadius={5}
        position="relative"
        transition="top ease .1s"
        top={0}
        _hover={{ boxShadow: "dark-lg", top: "-3px", bg: bg.hover }}
      >
        <Heading as="h3" size="md" fontWeight="medium" py={2}>
          {heading}
        </Heading>
        {children}
      </VStack>
    </Link>
  );
};
