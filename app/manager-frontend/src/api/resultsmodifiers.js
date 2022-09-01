import { useBackendApi } from "contexts/BackendApi";

import useSWR from "swr";

export const fetchKeys = {
  modifierTypeList: "ResultsModifiers/Types",
  modifierOrderList: (id, value) => `${id}/Order/${value}`
};

export const getResultsModifierApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("ResultsModifiers", {
      json: values,
    }),
  update: ({ values, id }) =>
    api.put(`ResultsModifiers/${id}`, {
      json: values,
    }),
  delete: ({ values, id }) =>
    api.delete(`ResultsModifiers/${id}`, {
      json: values,
    }),
  putOrder: ({ value, id }) =>
    api.put(`${id}/order/${value}`)
});

export const useResultsModifierOrderList = (id, value) => {
  const { apiFetcher } = useBackendApi();
  return useSWR(fetchKeys.modifierOrderList(id, value), apiFetcher, { suspense: true });
};

export const useModifierTypeList = () => {
  const { apiFetcher } = useBackendApi();
  return useSWR(fetchKeys.modifierTypeList, apiFetcher, { suspense: true });
};
