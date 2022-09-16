import React from "react";
import { ChakraProvider, extendTheme } from "@chakra-ui/react";

const chakraTheme = extendTheme({
  components: {
    Heading: {
      baseStyle: { m: 0 },
    },
    Link: {
      baseStyle: { color: "blue.600" },
    },
    Button: {
      baseStyle: {
        borderStyle: "inherit",
      },
      sizes: {
        xl: {
          h: 16,
          minW: 20,
          fontSize: "2xl",
          px: 8,
        },
      },
    },
  },
  styles: {
    global: {
      body: {
        bg: "inherit",
      },
    },
  },
});

const Root = ({ children }) => {
  return (
    <ChakraProvider resetCSS={false} theme={chakraTheme}>
      {children}
    </ChakraProvider>
  );
};

export default Root;
