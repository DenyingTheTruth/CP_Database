import React from "react";
import ReportView from "../../components/Content/Common/ReportView/ReportView";
import { useLocation } from "react-router-dom";

const CorrectBalanceSheetPage = () => {
  const location = useLocation();
  const year = location.search.slice(6);
  const reportId = location.pathname.slice(17);

  return <ReportView correct={true} year={year} reportId={reportId} />;
};

export default CorrectBalanceSheetPage;
