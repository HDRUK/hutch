import { createContext, useContext, useEffect, useMemo, useState } from "react";
import Cookies from "js-cookie";
import { useProfile } from "api/user";
import { useTranslation } from "react-i18next";

const UserContext = createContext({});

export const useUser = () => useContext(UserContext);

const getCookieProfile = () => {
  const yum = Cookies.get(".LinkLiteManager.Profile");
  return yum ? JSON.parse(yum) : null;
};

/**
 * Checks User Status on app load,
 * and provides methods to sign a user in and out
 * in response to app events (e.g. Login/Logout)
 */
export const UserProvider = ({ children }) => {
  const { i18n } = useTranslation();
  const [user, setUser] = useState(getCookieProfile());

  const { data: profile, mutate } = useProfile();

  useEffect(() => {
    setUser(profile);
  }, [profile]);

  useEffect(() => {
    user && i18n.changeLanguage(user.uiCulture);
  }, [user]);

  const signOut = () => setUser(null);
  const updateProfile = () => mutate();

  const context = useMemo(
    () => ({ user, signIn: setUser, signOut, updateProfile }),
    [user, setUser, signOut, updateProfile]
  );

  return (
    <UserContext.Provider value={context}>{children}</UserContext.Provider>
  );
};
