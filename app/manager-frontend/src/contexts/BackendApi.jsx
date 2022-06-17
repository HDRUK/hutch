import { getAccountApi } from "api/account";
import { getUserApi } from "api/user";
import { getActivitySourceApi } from "api/activitysource";
import ky from "ky";
import { createContext, useCallback, useContext, useMemo } from "react";
import { useTranslation } from "react-i18next";
const BackendApiContext = createContext({});
export const useBackendApi = () => useContext(BackendApiContext);

/** Default KY instance options for hitting the backend API */
export const getBackendDefaults = (language) => ({
  prefixUrl: "/api/",
  headers: {
    "Accept-Language": language,
  },
});

export const BackendApiProvider = ({ children }) => {
  // guarantees i18n is initialised
  // and gives us the instance so we can
  // tell the backend the language in use :)
  const { i18n } = useTranslation();

  // preconfigured ky instance for hitting the backend api
  const api = useMemo(
    () => ky.create(getBackendDefaults(i18n.language)),
    [i18n]
  );

  /**
   * A default fetcher for SWR to get data from the backend API
   * @param {*} path the url path relative to `https://{backend}/api/`
   * @returns
   */
  const apiFetcher = useCallback(
    async (path) => await api.get(path).json(),
    [api]
  );

  const baseContext = useMemo(() => ({ api, apiFetcher }), [api, apiFetcher]);

  const context = useMemo(
    () => ({
      ...baseContext,
      account: getAccountApi(baseContext),
      users: getUserApi(baseContext),
      activitysource: getActivitySourceApi(baseContext),
    }),
    [baseContext]
  );

  return (
    <BackendApiContext.Provider value={context}>
      {children}
    </BackendApiContext.Provider>
  );
};
