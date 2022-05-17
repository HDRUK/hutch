import { Grid, VStack } from "@chakra-ui/react";
import { Outlet } from "react-router-dom";
import { NavBar } from "components/NavBar";
import { useLocationStateToast } from "helpers/hooks/useLocationStateToast";
import { Footer } from "components/Footer";

export const DefaultLayout = ({ toastDefaults = { position: "top" } }) => {
  useLocationStateToast(toastDefaults);

  return (
    <Grid templateRows="auto 1fr" height="100vh">
      <NavBar />

      <VStack overflow="auto" w="100%">
        <Grid templateRows="1fr auto" minHeight="100%" w="100%">
          <Outlet />

          <Footer />
        </Grid>
      </VStack>
    </Grid>
  );
};
