import React from "react";
import { Route, Switch } from "react-router-dom";
import BalanceSheetUser from "../../components/Content/Common/AnalyticsContent/BalanceSheet/BalanceSheetUser";

const AnalyticsPage = () => {
  return (
    <Switch>
      <Route
        exact
        path="/analytics/balance-sheet"
        component={() => <BalanceSheetUser />}
      />
    </Switch>
  );
};
export default AnalyticsPage;
