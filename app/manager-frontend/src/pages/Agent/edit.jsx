import { useBackendApi } from "contexts/BackendApi";
import { Agent } from ".";
import { useParams } from "react-router-dom";
import { useAgent } from "api/agents";

function edit() {
  const {
    agent: { update },
  } = useBackendApi();
  const { id } = useParams();
  const { data } = useAgent(id);
  return <Agent action={update} id={id} data={data} />;
}
export default edit;
