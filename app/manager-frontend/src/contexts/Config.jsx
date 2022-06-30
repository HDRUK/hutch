import { createContext, useContext, useState } from "react";
import Cookies from "js-cookie";

const BackendConfigContext = createContext({});

export const useBackendConfig = () => useContext(BackendConfigContext);

const getCookieConfig = () => {
  const yum = Cookies.get(".HutchManager.Config");
  return yum ? JSON.parse(yum) : null;
};

export const BackendConfigProvider = ({ children }) => {
  const [config, setConfig] = useState(getCookieConfig());

  const context = { config };

  return (
    <BackendConfigContext.Provider value={context}>
      {children}
    </BackendConfigContext.Provider>
  );
};
