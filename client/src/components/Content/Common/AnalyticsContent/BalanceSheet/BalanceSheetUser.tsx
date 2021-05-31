import React, { useState, useEffect } from "react";
import { Select, DatePicker, Button, TreeSelect, Tabs, Empty } from "antd";
import moment, { Moment } from "moment";
import { ExportOutlined } from "@ant-design/icons";

import AnalysisOfTheBalanceSheetAssetStructure from "./Reports/AnalysisOfTheBalanceSheetAssetStructure";
import AnalysisOfTheStructureOfTheLiabilityBalance from "./Reports/AnalysisOfTheStructureOfTheLiabilityBalance";
import AnalysisOfFinancialIndicators from "./Reports/AnalysisOfFinancialIndicators";
import AnalysisOfTheStructureOfLiabilities from "./Reports/AnalysisOfTheStructureOfLiabilities";
import SolvencyRatios from "./Reports/SolvencyRatios";
import ConditionOfOwnWorkingCapital from "./Reports/ConditionOfOwnWorkingCapital";

import { getReportTypeByReportName } from "../../../../../services/directories/directories.services";

import { NestedPeriod, Filter } from "../../../../../models/common-types";

import {
  getBalanceAsset,
  getBalanceLiabilities,
  getFinancialIndicators,
  getStructureLiabilities,
  getSolvencyRatios,
  getWorkingCapital,
  exportAnalytics,
} from "../../../../../services/analytics/analytics.services";

import {
  analyticsReportSelect,
  analyticsFilter,
} from "../../../../../constants/constants";
import { useAuth } from "../../../../Auth/Auth";

const { SHOW_PARENT } = TreeSelect;
const { Option } = Select;
const { TabPane } = Tabs;

const BalanceSheetUser = () => {
  const { gYear } = useAuth();

  const [currReport, setCurrReport] = useState<string>("");
  const [currReportForLoader, setCurrReportForLoader] = useState<string>("");
  const [currReportId, setCurrReportId] = useState<string>("");
  const [currYear, setCurrYear] = useState<Moment | null>(gYear);
  const [reportPeriods, setReportPeriods] = useState<Array<NestedPeriod>>([]);
  const [currPeriod, setCurrPeriod] = useState<string>("");
  const [currFilter, setCurrFilter] = useState<Array<number>>([2, 6]);
  const [data, setData] = useState<any | undefined>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [exportButtonDisabled, setExportButtonDisabled] = useState<boolean>(
    true,
  );

  useEffect(() => {
    (async function () {
      const reportPeriods = await getReportTypeByReportName(
        "Бухгалтерский баланс и приложения к нему",
      );
      setReportPeriods(reportPeriods?.periods ?? []);
      setCurrReportId(reportPeriods?.id ?? "");
    })();
  }, []);

  const onYearChange = (value: Moment | null) => {
    setCurrYear(value);
  };

  const onReportPeriodChange = (value: string) => {
    setCurrPeriod(value);
  };

  const onReportChange = (value: string) => {
    setCurrReport(value);
  };

  const onFilterChange = (value: Array<number>) => {
    setCurrFilter(value);
  };

  const analyticsRequest = async () => {
    setLoading(true);
    setCurrReportForLoader(currReport);
    const dataForRequest = {
      year: moment(currYear).year(),
      periodId: currPeriod,
      reportTypeId: currReportId,
      filter: currFilter,
    };
    const newData = await requestSwitch(currReport, dataForRequest);
    setData(newData);
    setLoading(false);
    setExportButtonDisabled(false);
  };

  const requestSwitch = async (currReport: string, dataForRequest: any) => {
    switch (currReport) {
      case "1":
        return await getBalanceAsset(dataForRequest, "user");
      case "2":
        return await getBalanceLiabilities(dataForRequest, "user");
      case "3":
        return await getFinancialIndicators(dataForRequest, "user");
      case "4":
        return await getStructureLiabilities(dataForRequest, "user");
      case "5":
        return await getSolvencyRatios(dataForRequest, "user");
      case "6":
        return await getWorkingCapital(dataForRequest, "user");
    }
  };

  const exportAnalyticsReport = async () => {
    setLoading(true);
    const dataForRequest = {
      year: moment(currYear).year(),
      periodId: currPeriod,
      reportTypeId: currReportId,
      filter: currFilter,
    };

    await exportAnalytics(Number(currReport) - 1, dataForRequest, "user");
    setLoading(false);
  };

  let children;

  switch (currReportForLoader) {
    case "1":
      children = (
        <Tabs type={"card"} defaultActiveKey={"1"}>
          <TabPane tab="Долгосрочные активы" key="1">
            <AnalysisOfTheBalanceSheetAssetStructure
              data={data?.tab1}
              tab={1}
              loading={loading}
              isAdmin={false}
            />
          </TabPane>
          <TabPane tab="Краткосрочные активы" key="2">
            <AnalysisOfTheBalanceSheetAssetStructure
              data={data?.tab2}
              tab={2}
              loading={loading}
              isAdmin={false}
            />
          </TabPane>
        </Tabs>
      ); //АНАЛИЗ СТРУКТУРЫ АКТИВА БАЛАНСА
      break;
    case "2":
      children = (
        <AnalysisOfTheStructureOfTheLiabilityBalance
          data={data?.tab1}
          loading={loading}
          isAdmin={false}
        />
      ); //АНАЛИЗ СТРУКТУРЫ ПАССИВА БАЛАНСА
      break;
    case "3":
      children = (
        <AnalysisOfFinancialIndicators
          data={data?.tab1}
          loading={loading}
          isAdmin={false}
        />
      ); //АНАЛИЗ ФИНАНСОВЫХ ПОКАЗАТЕЛЕЙ
      break;
    case "4":
      children = (
        <AnalysisOfTheStructureOfLiabilities
          data={data?.tab1}
          loading={loading}
          isAdmin={false}
        />
      ); //АНАЛИЗ СТРУКТУРЫ ОБЯЗАТЕЛЬСТВ
      break;
    case "5":
      children = (
        <SolvencyRatios data={data?.tab1} loading={loading} isAdmin={false} />
      ); //КОЭФФИЦИЕНТЫ ПЛАТЕЖЕСПОСОБНОСТИ
      break;
    case "6":
      children = (
        <ConditionOfOwnWorkingCapital
          data={data?.tab1}
          loading={loading}
          isAdmin={false}
        />
      ); //СОСТОЯНИЕ СОБСТВЕННЫХ ОБОРОТНЫХ СРЕДСТВ
      break;
    default:
      children = (
        <Empty
          style={{ minHeight: "calc(100vh - 150px)", backgroundColor: "#fff" }}
          className={"flex-row --row-center --flex-col "}
        />
      );
  }

  return (
    <>
      <div
        className="flex-row --row-start gap-20 mb-24"
        style={{ flexFlow: "row wrap" }}
      >
        <div className="flex-row --row-start gap-20">
          <Select
            onChange={onReportChange}
            placeholder="Выберите аналитический отчет"
          >
            {analyticsReportSelect?.map((item: Filter) => (
              <Option key={item.value} value={item.value}>
                {item.text}
              </Option>
            ))}
          </Select>
          <DatePicker
            value={currYear}
            picker="year"
            onChange={onYearChange}
            style={{ width: "150px" }}
          />
          <Select
            style={{ width: "260px" }}
            onChange={onReportPeriodChange}
            placeholder="Выберите период"
          >
            {reportPeriods?.map((item: NestedPeriod) => (
              <Option key={item.id} value={item.id}>
                {item.name}
              </Option>
            ))}
          </Select>
        </div>
        <div className="flex-row --row-start gap-20">
          <Button
            disabled={
              !(currYear && currPeriod && currFilter?.toString() && currReport)
            }
            onClick={analyticsRequest}
          >
            Применить
          </Button>
          <span className={"fixed-drawer-button --export"}>
            <Button
              type="primary"
              title={"Экспорт аналитического отчета"}
              icon={<ExportOutlined style={{ fontSize: "24px" }} />}
              disabled={exportButtonDisabled}
              onClick={exportAnalyticsReport}
            />
          </span>
        </div>
      </div>
      <div>{children}</div>
    </>
  );
};

export default BalanceSheetUser;
