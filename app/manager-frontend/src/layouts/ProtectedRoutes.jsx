import { BusyPage } from "components/Busy";
import { useUser } from "contexts/User";
import { Suspense, useEffect } from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";

// This does the hard work asynchronously, using Suspense while busy
export const RequireAuth = ({ isAuthorized = () => true }) => {
  const { user } = useUser();
  const location = useLocation();
  const navigate = useNavigate();

  let authorized = !!user && isAuthorized(user);

  useEffect(() => {
    const authorized = !!user && isAuthorized(user);
    if (!authorized)
      // Redirect them to the /login page, but save the current location they were
      // trying to go to when they were redirected. This allows us to send them
      // along to that page after they login, which is a nicer user experience
      // than dropping them off on the home page.
      navigate("/account/login", {
        state: {
          from: location,
        },
      });
  }, [user]);

  return authorized ? <Outlet /> : null;
};

/**
 * A layout within which routes are protected.
 *
 * Default protection requires an authenticated user only
 *
 * A Custom authorisation function (which will be passed the authenticated user's profile)
 * which returns a bool (true if the user is authorized) can be provided
 * @param {*} param0
 * @returns
 */
export const ProtectedRoutes = ({ isAuthorized = () => true }) => (
  <Suspense fallback={<BusyPage />}>
    <RequireAuth isAuthorized={isAuthorized} />
  </Suspense>
);
