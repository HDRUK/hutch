import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  activitySourceList: "ActivitySource",
  activitySource: (id) => `ActivitySource/${id}`,
};

export const getActivitySourceApi = ({ api }) => ({
  /**
   * Create new activity source
   * @param {*} body
   */
  create: ({ values }) =>
    api.post("ActivitySource", {
      json: values,
    }),
  update: ({ values, id }) =>
    api.put(`ActivitySource/${id}`, {
      json: values,
    }),
  delete: ({ values, id }) =>
    api.delete(`ActivitySource/${id}`, {
      json: values,
    }),
});

export const useActivitySourceList = () => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.activitySourceList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useActivitySource = (id) => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.activitySource(id),
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
