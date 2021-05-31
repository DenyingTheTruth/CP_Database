import React, { ReactText } from "react";
import { Moment } from "moment";

import { Button, DatePicker, Select } from "antd";

import { IOrgForCopy } from "../../../../models/common-interfaces";
import { ReportType } from "../../../../models/common-types";

import "./TableManagementPanel.scss";

const { Option } = Select;

export const TableManagementPanel = ({
  currYear,
  onYearStateChange,
  reportTypes,
  onReportTypeStateChange,
  orgsForCopy,
  orgForCopyFrom,
  onOrgsForCopyFromChange,
  orgsForCopyTo,
  ableToCopy,
  onCopyingPeriods,
  tableLoading,
  tableEdit,
}: {
  currYear: Moment | null;
  onYearStateChange: (value: Moment | null) => void;
  reportTypes: Array<ReportType>;
  onReportTypeStateChange: (value: string) => void;
  orgsForCopy: Array<IOrgForCopy>;
  orgForCopyFrom: string | undefined;
  orgsForCopyTo: ReactText[];
  onOrgsForCopyFromChange: (value: string) => void;
  ableToCopy: boolean;
  onCopyingPeriods: () => void;
  tableLoading: (value: boolean) => void;
  tableEdit: boolean;
}) => {
  const onYearChange = (value: Moment | null): void => {
    onYearStateChange(value);
  };

  const onReportTypeChange = (value: string) => {
    onReportTypeStateChange(value);
  };

  const onOrgChange = (value: string) => {
    onOrgsForCopyFromChange(value);
  };

  const onCopyingPeriodsClick = () => {
    tableLoading(true);
    onCopyingPeriods();
  };

  return (
    <div className={"flex-row mb-24 gap-20"} style={{ flexFlow: "row wrap" }}>
      <div className={"flex-row --row-start gap-20"}>
        <DatePicker
          className={"deadlines__calendar"}
          value={currYear}
          picker="year"
          onChange={onYearChange}
          disabled={tableEdit || !!orgForCopyFrom}
        />
        <Select
          onChange={onReportTypeChange}
          placeholder={"Выбрать тип отчета для просмотра..."}
          disabled={tableEdit || !!orgForCopyFrom}
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
      </div>
      <div className={"flex-row --row-end gap-20"}>
        <Select
          placeholder={"Выберите организацию для копирования сроков..."}
          onChange={onOrgChange}
          allowClear={true}
          value={orgForCopyFrom}
          disabled={tableEdit}
        >
          {orgsForCopy.map((item) => (
            <Option key={item.organizationId} value={item.organizationId}>
              {item.organizationName}
            </Option>
          ))}
        </Select>
        <Button
          onClick={onCopyingPeriodsClick}
          disabled={!ableToCopy || !orgsForCopyTo.length}
        >
          Копировать
        </Button>
      </div>
    </div>
  );
};
