import { ChakraProvider, Flex, Spinner } from "@chakra-ui/react";
import { StrictMode, Suspense } from "react";
import ReactDOM from "react-dom";
import { theme } from "themes/theme";

import "config/i18n";
import { BrowserRouter } from "react-router-dom";
import { Root } from "routes/Root";
import { UserProvider } from "contexts/User";
import { ErrorBoundary } from "components/ErrorBoundary";
import { BackendApiProvider } from "contexts/BackendApi";
import { BackendConfigProvider } from "contexts/Config";

ReactDOM.render(
  <StrictMode>
    <ChakraProvider theme={theme}>
      <BrowserRouter>
        <ErrorBoundary>
          <Suspense
            fallback={
              <Flex justify="center" w="100%" my={16}>
                <Spinner boxSize={16} />
              </Flex>
            }
          >
            <BackendApiProvider>
              <UserProvider>
                <BackendConfigProvider>
                  <Root />
                </BackendConfigProvider>
              </UserProvider>
            </BackendApiProvider>
          </Suspense>
        </ErrorBoundary>
      </BrowserRouter>
    </ChakraProvider>
  </StrictMode>,
  document.getElementById("root")
);
