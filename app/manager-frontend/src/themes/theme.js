import { extendTheme, theme as defaultTheme } from "@chakra-ui/react";
export const theme = extendTheme({
  config: {
    initialColorMode: "light",
    useSystemColorMode: false,
  },
  components: {
    Input: {
      defaultProps: {
        variant: "filled",
      },
    },
    Textarea: {
      defaultProps: {
        variant: "filled",
      },
    },
    Container: {
      baseStyle: {
        maxWidth: "100ch",
      },
    },
    Heading: {
      baseStyle: {
        fontWeight: "medium",
      },
    },
    Link: {
      baseStyle: { color: "blue.600" },
    },
    Editable: {
      variants: {
        flushed: {
          input: (p) => defaultTheme.components.Input.variants.flushed(p).field,
        },
      },
    },
  },
  shadows: {
    callout: "0 2px 10px 0 rgba(0,0,0,.132), 0 0.5px 1.8px 0 rgba(0,0,0,.108)",
    "section-h": "0 2px 15px 0 rgba(0,0,0,.2), 0 1px 3px 0 rgba(0,0,0,.5)",
    "section-v": "2px 0 10px 0 rgba(0,0,0,.2), 1px 0 3px 0 rgba(0,0,0,.5)",
  },
});
