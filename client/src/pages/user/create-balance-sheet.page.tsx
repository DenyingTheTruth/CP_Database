import React from "react";
import { useLocation } from "react-router-dom";
import ReportView from "../../components/Content/Common/ReportView/ReportView";

const CreateBalanceSheetPage = () => {
  const location = useLocation();
  const year = location.search.slice(6, 10);

  return <ReportView check={false} year={year} />;
};

export default CreateBalanceSheetPage;
