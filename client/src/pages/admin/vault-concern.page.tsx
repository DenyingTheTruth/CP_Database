import React, { useState, useEffect } from "react";
import { Select, DatePicker, TreeSelect, Button } from "antd";
import moment, { Moment } from "moment";

import ReportView from "../../components/Content/Common/ReportView/ReportView";

import { vaultFilter } from "../../constants/constants";
import { getReportTypes } from "../../services/directories/directories.services";
import { ReportType, NestedPeriod } from "../../models/common-types";
import { useAuth } from "../../components/Auth/Auth";

const { Option } = Select;

const VaultConcernPage = () => {
  const { gYear } = useAuth();

  const [reportTypes, setReportTypes] = useState<Array<ReportType>>([]);
  const [reportPeriods, setReportPeriods] = useState<
    Array<NestedPeriod> | undefined
  >([]);
  const [currReportType, setCurrReportType] = useState<string>("");
  const [currYear, setCurrYear] = useState<Moment | null>(gYear);
  const [currPeriod, setCurrPeriod] = useState<string>("");
  const [currFilter, setCurrFilter] = useState<number | null>(null);
  const [vaultReq, setVaultReq] = useState<boolean | undefined>();
  const [vaultConcernData, setVaultConcernData] = useState<any>(null);
  const [disableApply, setDisableApply] = useState<boolean>(false);

  useEffect(() => {
    (async function () {
      const newReportTypes = await getReportTypes();
      setReportTypes(newReportTypes);
    })();
  }, []);

  const onReportTypeChange = (value: string) => {
    const currReportType = reportTypes.find(
      (item: ReportType) => item.id === value,
    );
    const newReportPeriods = currReportType?.periods;
    setReportPeriods(newReportPeriods);
    setCurrReportType(value);
  };

  const onYearChange = (value: Moment | null) => {
    setCurrYear(value);
  };

  const onReportPeriodChange = (value: string) => {
    setCurrPeriod(value);
  };

  const onFilterChange = (value: number) => {
    setCurrFilter(value);
  };

  const vaultRequest = async () => {
    const vaultReport = {
      year: moment(currYear).year(),
      periodId: currPeriod,
      reportTypeId: currReportType,
      filter: [currFilter],
    };
    setVaultConcernData(vaultReport);
    setVaultReq(!vaultReq);
    toggleApply(true);
  };

  const tProps = {
    treeData: vaultFilter,
    onChange: onFilterChange,
    placeholder: "Выберите категорию организаций",
    treeDefaultExpandAll: true,
    style: {
      width: "350px",
    },
    listHeight: 512,
  };

  const toggleApply = (value: boolean) => {
    setDisableApply(value);
  };

  return (
    <div className="vault-wrap">
      <div
        className="flex-row --row-start gap-20 mb-24"
        style={{ flexFlow: "row wrap" }}
      >
        <div className="flex-row --row-start gap-20">
          <Select
            onChange={onReportTypeChange}
            placeholder={"Выберите тип отчета"}
          >
            {reportTypes.map(
              (item) =>
                item.id !== "69a1be33-438c-4718-be23-fa842c2545d9" && (
                  <Option key={item.id} value={item.id}>
                    {item.name}
                  </Option>
                ),
            )}
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
              <Option
                key={item.id}
                value={item.id}
                disabled={item.id === "69a1be33-438c-4718-be23-fa842c2545d9"}
              >
                {item.name}
              </Option>
            ))}
          </Select>
        </div>
        <div className="flex-row gap-20">
          <TreeSelect {...tProps} />
          <Button
            disabled={
              !(
                currReportType &&
                currYear &&
                currPeriod &&
                currFilter?.toString()
              ) || disableApply
            }
            onClick={vaultRequest}
          >
            Применить
          </Button>
        </div>
      </div>
      <ReportView
        vault={true}
        vaultReq={vaultReq}
        vaultData={vaultConcernData}
        toggleApply={toggleApply}
      />
    </div>
  );
};

export default VaultConcernPage;
