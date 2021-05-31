import React from "react";
import { Switch, Route } from "react-router-dom";

import AdminHomeTable from "../../components/Content/Admin/AdminTable/AdminReportsTable";
import CheckBalanceSheetPage from "./check-balance-sheet.page";

const AdminHomePage = () => {
  return (
    <Switch>
      <Route exact path="/">
        <AdminHomeTable />
      </Route>
      <Route path={`/check-report/:id`}>
        <CheckBalanceSheetPage />
      </Route>
    </Switch>
  );
};

export default AdminHomePage;
