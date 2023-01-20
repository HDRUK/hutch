import { Route, useNavigate } from "react-router-dom";
import { Routes } from "react-router-dom";
import { DefaultLayout } from "layouts/DefaultLayout";
import { Account } from "./Account";
import { NotFound } from "pages/error/NotFound";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { UserHome } from "pages/UserHome";
import { useUser } from "contexts/User";
import { useEffect } from "react";
import { ContentPage } from "pages/ContentPage";
import { useBackendConfig } from "contexts/Config";
import CreateActivitySource from "pages/ActivitySource/create";
import EditActivitySource from "pages/ActivitySource/edit";
import { DataSourcesList } from "pages/DataSource/list";
import RegisterAgent from "pages/Agent/create";
import RegisterUser from "pages/User/create";
import EditAgent from "pages/Agent/edit";

const IndexRedirect = () => {
  const { user } = useUser();
  const navigate = useNavigate();
  useEffect(() => {
    const targetPath = user ? "/home" : "/account/login";
    navigate(targetPath, { replace: true });
  }, [user]);

  return null;
};

export const Root = () => {
  const { config } = useBackendConfig(); // TODO use for feature flagging routes
  return (
    <Routes>
      <Route path="/" element={<DefaultLayout />}>
        <Route index element={<IndexRedirect />} />

        <Route path="about" element={<ContentPage contentKey={"about"} />} />
        <Route path="/" element={<ProtectedRoutes />}>
          <Route path="home" element={<UserHome />} />
          <Route path="home/:listname" element={<UserHome />} />
          <Route
            path="activitysources/new"
            element={<CreateActivitySource />}
          />
          <Route path="agents/new" element={<RegisterAgent />} />
          <Route path="datasources" element={<DataSourcesList />} />
          <Route path="activitysources/:id" element={<EditActivitySource />} />
          <Route path="agents/:id" element={<EditAgent />} />
          <Route path="users/new" element={<RegisterUser />} />
        </Route>

        <Route path="account/*" element={<Account />} />

        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
};
