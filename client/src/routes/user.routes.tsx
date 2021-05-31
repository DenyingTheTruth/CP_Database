import React from "react";
import { Redirect, Route, Switch } from "react-router-dom";
import HistoryReportPage from "../pages/user/history-reports.page";
import CreateBalanceSheetPage from "../pages/user/create-balance-sheet.page";
import CorrectReportPage from "../pages/user/correct-reports.page";
import UserGuidePage from "../pages/user/user-guide.page";
import HomePage from "../pages/user/homePage/home.page";
import GeneralInformationPage from "../pages/user/general-information.page";
import AnalyticsPage from "../pages/user/analytics.page";

const UserRoutes = () => {
  return (
    <Switch>
      <Route
        path="/history-send-reports"
        component={() => <HistoryReportPage />}
      />
      <Route
        exact
        path="/create-balance-sheet"
        component={() => <CreateBalanceSheetPage />}
      />
      <Route path="/correct-reports" component={() => <CorrectReportPage />} />
      <Route path="/analytics" component={() => <AnalyticsPage />} />
      <Route
        exact
        path="/general-information"
        component={() => <GeneralInformationPage />}
      />
      <Route exact path="/user-guide" component={() => <UserGuidePage />} />
      <Route exact path="/" component={() => <HomePage />} />
      <Redirect to="/" />
    </Switch>
  );
};

export default UserRoutes;
