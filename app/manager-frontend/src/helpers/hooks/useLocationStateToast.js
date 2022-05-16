import { useEffect } from "react";
import { useToast } from "@chakra-ui/react";
import { useLocation } from "react-router-dom";
import merge from "lodash-es/merge";

export const useLocationStateToast = (defaults = {}) => {
  const toast = useToast();
  const { state } = useLocation();
  useEffect(() => {
    if (state?.toast) toast(merge({}, defaults, state.toast));
  }, [state, toast]);
};
