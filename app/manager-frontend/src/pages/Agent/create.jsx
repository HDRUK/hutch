import { useBackendApi } from "contexts/BackendApi";
import { Agent } from ".";

function create() {
  const {
    agent: { create },
  } = useBackendApi();
  return <Agent action={create} />;
}

export default create;
