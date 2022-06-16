import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  activitySourceList: "ActivitySource",
};

export const getActivitySourceApi = ({ api }) => ({
  /**
   * Create new activity source
   * @param {*} body
   */
  create: (values) =>
    api.post("ActivitySource", {
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
