import React from "react";
import { ChakraProvider, extendTheme } from "@chakra-ui/react";

const Root = ({ children }) => {
  return (
    <ChakraProvider
      resetCSS={false}
      theme={extendTheme({
        components: {
          Heading: {
            baseStyle: { m: 0 },
          },
          Link: {
            baseStyle: { color: "blue.600" },
          },
        },
        styles: {
          global: {
            body: {
              bg: "inherit",
            },
          },
        },
      })}
    >
      {children}
    </ChakraProvider>
  );
};

export default Root;
