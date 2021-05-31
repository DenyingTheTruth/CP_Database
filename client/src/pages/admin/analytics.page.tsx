import React from "react";
import { Route, Switch } from "react-router-dom";
import BalanceSheetAdmin from "../../components/Content/Common/AnalyticsContent/BalanceSheet/BalanceSheetAdmin";

const AnalyticsPage = () => {
  return (
    <Switch>
      <Route
        exact
        path="/analytics/balance-sheet"
        component={() => <BalanceSheetAdmin />}
      />
    </Switch>
  );
};

export default AnalyticsPage;
