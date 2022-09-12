import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  activitySourceList: "ActivitySources",
  activitySource: (id) => `ActivitySources/${id}`,
  resultsModifiers: (id) => `ActivitySources/${id}/ResultsModifiers`
};

export const getActivitySourceApi = ({ api }) => ({
  /**
   * Create new activity source
   * @param {*} body
   */
  create: ({ values }) =>
    api.post("ActivitySources", {
      json: values,
    }),
  update: ({ values, id }) =>
    api.put(`ActivitySources/${id}`, {
      json: values,
    }),
  delete: ({ values, id }) =>
    api.delete(`ActivitySources/${id}`, {
      json: values,
    }),
  createModifier: ({ values, id }) =>
    api.post(`ActivitySources/${id}/ResultsModifiers`, {
      json: values,
    })
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

export const useActivitySourceResultsModifiersList = (id) => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.resultsModifiers(id),
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
