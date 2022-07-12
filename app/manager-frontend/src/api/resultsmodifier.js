import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  resultsModifierList: "ResultsModifier",
};

export const getResultsModifierApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("ResultsModifier", {
      json: values,
    }),
  update: ({ values, id }) =>
    api.put(`ResultsModifier/${id}`, {
      json: values,
    }),
  delete: ({ values, id }) =>
    api.delete(`ResultsModifier/${id}`, {
      json: values,
    }),
});

export const useResultsModifierList = () => {
  const { apiFetcher } = useBackendApi();
  return useSWR(fetchKeys.resultsModifierList, apiFetcher, { suspense: true });
};
