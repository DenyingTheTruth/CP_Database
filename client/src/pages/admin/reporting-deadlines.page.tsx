import React, { useEffect, useState, ReactText } from "react";
import moment, { Moment } from "moment";

import { TableManagementPanel } from "../../components/Content/Admin/ReportingDeadlines/TableManagementPanel";
import { ReportingDeadlinesTable } from "../../components/Content/Admin/ReportingDeadlines/ReportingDeadlinesTable";

import { getReportTypes } from "../../services/directories/directories.services";
import {
  getIntervals,
  changeInterval,
  copyInterval,
} from "../../services/interval/interval.services";

import { IIntervals, IOrgForCopy } from "../../models/common-interfaces";
import { ReportType } from "../../models/common-types";
import { Beforeunload } from "react-beforeunload";
import { useAuth } from "../../components/Auth/Auth";

const ReportingDeadlinesPage = () => {
  const { gYear } = useAuth();

  const [reportTypes, setReportTypes] = useState<Array<ReportType>>([]);
  const [currYear, setCurrYear] = useState<Moment | null>(gYear);
  const [currReportType, setCurrReportType] = useState<string>("");
  const [reportInfo, setReportInfo] = useState<ReportType>();
  const [intervals, setIntervals] = useState<Array<IIntervals>>([]);
  const [orgsForCopy, setOrgsForCopy] = useState<Array<IOrgForCopy>>([]);
  const [orgForCopyFrom, setOrgForCopyFrom] = useState<string | undefined>(
    undefined,
  );
  const [orgsForCopyTo, setOrgsForCopyTo] = useState<ReactText[]>([]);
  const [updateTable, setUpdateTable] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [ableToCopy, setAbleToCopy] = useState<boolean>(false);
  const [tableEdit, setTableEdit] = useState<boolean>(false);

  useEffect(() => {
    (async function () {
      const newReportTypes = await getReportTypes();
      setReportTypes(newReportTypes);
    })();
  }, []);

  useEffect(() => {
    if (currYear && currReportType) {
      (async function () {
        setLoading(true);
        const currReportInfo = await getReportTypes(currReportType);
        const newIntervals: Array<IIntervals> = await getIntervals(
          moment(currYear).year(),
          currReportType,
        );
        setIntervals(newIntervals);
        setReportInfo(currReportInfo);
        const newArr = newIntervals.map((item) => {
          return {
            organizationId: item.organizationId,
            organizationName: item.organization.name,
          };
        });
        setOrgsForCopy(newArr);
      })();
    }
  }, [currYear, currReportType, updateTable]);

  useEffect(() => {
    setAbleToCopy(!!orgForCopyFrom);
  }, [orgForCopyFrom]);

  const onYearChange = (value: Moment | null) => {
    setCurrYear(value);
  };

  const onReportTypeChange = (value: string) => {
    setCurrReportType(value);
  };

  const onOrgForCopyFromChange = (value: string) => {
    setOrgForCopyFrom(value);
    setOrgsForCopyTo([]);
  };

  const onOrgsForCopyToChange = (value: ReactText[]) => {
    setOrgsForCopyTo(value);
  };

  const onTableRowSave = async (data: any) => {
    const periods = Object.values(data);
    const dataForReq = {
      year: moment(currYear).year(),
      organizationId: data.key,
      periods: periods.splice(2),
    };
    const result = await changeInterval(dataForReq);
    if (result.isSuccess) {
      setUpdateTable(!updateTable);
    }
  };

  const onCopyingPeriods = async () => {
    const dataForReq = {
      year: moment(currYear).year(),
      reportTypeId: currReportType,
      fromOrganizationId: orgForCopyFrom,
      toOrganizations: orgsForCopyTo,
    };
    const result = await copyInterval(dataForReq);
    if (result.isSuccess) {
      setUpdateTable(!updateTable);
    }
    setOrgForCopyFrom(undefined);
    setOrgsForCopyTo([]);
  };

  const tableLoading = (value: boolean) => {
    setLoading(value);
  };

  const isTableEdit = (value: boolean) => {
    setTableEdit(value);
  };

  return (
    <Beforeunload
      onBeforeunload={() =>
        "Не сохраненные данные будут потеряны. Вы уверены, что хотите покинуть страницу ?"
      }
    >
      <TableManagementPanel
        currYear={currYear}
        onYearStateChange={onYearChange}
        reportTypes={reportTypes}
        onReportTypeStateChange={onReportTypeChange}
        orgsForCopy={orgsForCopy}
        orgsForCopyTo={orgsForCopyTo}
        orgForCopyFrom={orgForCopyFrom}
        onOrgsForCopyFromChange={onOrgForCopyFromChange}
        ableToCopy={ableToCopy}
        onCopyingPeriods={onCopyingPeriods}
        tableLoading={tableLoading}
        tableEdit={tableEdit}
      />
      <ReportingDeadlinesTable
        intervals={intervals}
        reportInfo={reportInfo}
        onTableRowSave={onTableRowSave}
        tableLoading={tableLoading}
        isTableLoading={loading}
        orgForCopyFrom={orgForCopyFrom}
        orgsForCopyTo={orgsForCopyTo}
        onOrgsForCopyToChange={onOrgsForCopyToChange}
        isTableEdit={isTableEdit}
      />
    </Beforeunload>
  );
};

export default ReportingDeadlinesPage;
