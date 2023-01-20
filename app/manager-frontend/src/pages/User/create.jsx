import { useBackendApi } from "contexts/BackendApi";
import { User } from ".";

function create() {
  const {
    users: { create },
  } = useBackendApi();
  return <User action={create} />;
}

export default create;
