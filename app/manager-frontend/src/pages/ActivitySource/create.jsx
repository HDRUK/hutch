import React from "react";
import { useBackendApi } from "contexts/BackendApi";
import { ActivitySource } from ".";

function create() {
  const {
    activitysource: { create },
  } = useBackendApi();
  return <ActivitySource action={create} />;
}

export default create;
