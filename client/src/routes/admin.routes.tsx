import React from "react";
import { Redirect, Route, Switch } from "react-router-dom";

// pages
import AcceptedReportsPage from "../pages/admin/accepted-reports.page";
import VaultConcernPage from "../pages/admin/vault-concern.page";
import AnalyticsPage from "../pages/admin/analytics.page";
import ReportingDeadlinesPage from "../pages/admin/reporting-deadlines.page";
import OrganizationsPage from "../pages/admin/organizations.page";
import UsersPage from "../pages/admin/users.page";
import AdminGuidePage from "../pages/admin/admin-guide.page";
import AdminHomePage from "../pages/admin/admin-home.page";

const AdminRoutes = () => {
  return (
    <Switch>
      <Route
        exact
        path="/accepted-reports"
        component={() => <AcceptedReportsPage />}
      />
      <Route
        exact
        path="/vault-concern"
        component={() => <VaultConcernPage />}
      />
      <Route path="/analytics" component={() => <AnalyticsPage />} />
      <Route
        exact
        path="/reporting-deadlines"
        component={() => <ReportingDeadlinesPage />}
      />
      <Route
        exact
        path="/organizations"
        component={() => <OrganizationsPage />}
      />
      <Route exact path="/users" component={() => <UsersPage />} />
      <Route exact path="/admin-guide" component={() => <AdminGuidePage />} />
      <Route path="/" component={() => <AdminHomePage />} />
      <Redirect to="/" />
    </Switch>
  );
};

export default AdminRoutes;
