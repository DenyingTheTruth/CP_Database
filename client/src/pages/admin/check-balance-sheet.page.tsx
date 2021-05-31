import React from "react";
import { useLocation } from "react-router-dom";
import ReportView from "../../components/Content/Common/ReportView/ReportView";

const CheckBalanceSheetPage = () => {
  const location = useLocation();
  const year = location.search.slice(6);
  const reportId = location.pathname.slice(14);

  return <ReportView check={true} year={year} reportId={reportId} />;
};

export default CheckBalanceSheetPage;
