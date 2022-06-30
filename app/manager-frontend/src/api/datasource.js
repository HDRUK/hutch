import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  dataSourceList: "DataSources",
};

export const getDataSourceApi = ({ api }) => ({
  delete: ({ values, id }) =>
    api.delete(`DataSources/${id}`, {
      json: values,
    }),
});

export const useDataSourceList = () => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.dataSourceList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
