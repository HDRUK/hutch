import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  agentList: "Agents",
  agent: (id) => `Agents/${id}`,
};

export const getAgentApi = ({ api }) => ({
  /**
   * Register an agent
   * @param {*} body
   */
  create: ({ values }) =>
    api.post("Agents", {
      json: values,
    }),
  update: ({ values, id }) =>
    api.put(`Agents/${id}`, {
      json: values,
    }),
  delete: ({ values, id }) =>
    api.delete(`Agents/${id}`, {
      json: values,
    }),
  updateAgentSecret: ({ values, id }) =>
    api.put(`Agents/${id}/secret`, {
      json: values,
    }),
});

export const useAgentList = () => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.agentList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useAgent = (id) => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.agent(id),
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
