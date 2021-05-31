import React, { useState, useEffect, useRef } from "react";
import { Switch, Route, Link, useRouteMatch } from "react-router-dom";
import { Table, Button, Input, Space } from "antd";
import { SearchOutlined, EditFilled } from "@ant-design/icons";
import moment from "moment";
import Highlighter from "react-highlight-words";

import CorrectBalanceSheetPage from "./correct-balance-sheet.page";

import { getUserReportsRevision } from "../../services/report/report.services";

import { statusReportUser } from "../../constants/constants";
import { IUserReportTable } from "../../models/common-interfaces";
import { Filter } from "../../models/common-types";

const CorrectReportPage = () => {
  const [data, setData] = useState<Array<any>>([]);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [years, setYears] = useState<Array<Filter>>([]);
  const [periods, setPeriods] = useState<Array<Filter>>([]);

  let { path, url } = useRouteMatch();

  useEffect(() => {
    setLoading(true);
    (async function () {
      const getData = await getUserReportsRevision();
      const newData: Array<IUserReportTable> = [];
      getData.forEach((item: any, i: number) => {
        newData.push({
          index: i + 1,
          id: item.id,
          name: item.reportType.name,
          status: statusReportUser[item.statusReport].text,
          note: item.note,
          date: item.replyDate
            ? moment(item.replyDate).format("DD.MM.YYYY")
            : "-",
          year: item.userCheckinInterval.year,
          period: item.userCheckinInterval.period.name,
        });
      });
      const filterYears: Array<Filter> = [];
      const filterPeriods: Array<Filter> = [];
      const newYears: Array<number> = [];
      const newPeriods: Array<string> = [];
      newData.forEach((item: any) => {
        newYears.push(item.year);
      });
      newData.forEach((item: any) => {
        newPeriods.push(item.period);
      });
      const years: any = Array.from(new Set(newYears));
      const periods: any = Array.from(new Set(newPeriods));
      years.forEach((item: number) => {
        filterYears.push({
          text: item.toString(),
          value: item.toString(),
        });
      });
      periods.forEach((item: number) => {
        filterPeriods.push({
          text: item.toString(),
          value: item.toString(),
        });
      });
      setYears(filterYears);
      setPeriods(filterPeriods);
      setData(newData);
      setLoading(false);
    })();
  }, []);

  const inputRef = useRef<any>();

  const getColumnSearchProps = (dataIndex: string) => ({
    filterDropdown: ({
      setSelectedKeys,
      selectedKeys,
      confirm,
      clearFilters,
    }: {
      setSelectedKeys: (selectedKeys: any[]) => void;
      selectedKeys: any[];
      confirm: () => void;
      clearFilters: () => void;
    }) => (
      <div style={{ padding: 8 }}>
        <Input
          ref={inputRef}
          placeholder={`Поиск`}
          value={selectedKeys[0]}
          onChange={(e) =>
            setSelectedKeys(e.target.value ? [e.target.value] : [])
          }
          onPressEnter={() => handleSearch(selectedKeys, confirm, dataIndex)}
          style={{ width: 188, marginBottom: 8, display: "block" }}
        />
        <Space>
          <Button
            type="primary"
            onClick={() => handleSearch(selectedKeys, confirm, dataIndex)}
            icon={<SearchOutlined />}
            size="small"
            style={{ width: 90 }}
          >
            Поиск
          </Button>
          <Button
            onClick={() => handleReset(clearFilters)}
            size="small"
            style={{ width: 90 }}
          >
            Сброс
          </Button>
        </Space>
      </div>
    ),
    filterIcon: (filtered: any) => (
      <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
    ),
    onFilter: (value: any, record: any) => {
      return record[dataIndex]
        ? record[dataIndex]
            .toString()
            .toLowerCase()
            .includes(value.toLowerCase())
        : "";
    },
    onFilterDropdownVisibleChange: (visible: any) => {
      if (visible) {
        setTimeout(() => inputRef.current.focus(), 100);
      }
    },
    render: (text: string) => {
      return searchedColumn === dataIndex ? (
        <Highlighter
          highlightStyle={{ backgroundColor: "#ffc069", padding: 0 }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text.toString()}
        />
      ) : (
        text
      );
    },
  });

  const handleSearch = (
    selectedKeys: any[],
    confirm: any,
    dataIndex: string,
  ) => {
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex);
    confirm();
  };

  const handleReset = (clearFilters: () => void) => {
    clearFilters();
    setSearchText("");
  };

  const columns: any = [
    {
      title: "№",
      dataIndex: "index",
      key: "index",
      width: 70,
    },
    {
      title: "Название отчета",
      dataIndex: "name",
      key: "name",
      width: 350,
      ...getColumnSearchProps("name"),
    },
    {
      title: "Год",
      dataIndex: "year",
      key: "year",
      width: 90,
      filters: years,
      onFilter: (value: string, record: any) =>
        years.find((item: Filter) => item.value === value)?.value ===
        record.year.toString(),
    },
    {
      title: "Период сдачи отчетности",
      dataIndex: "period",
      key: "period",
      width: 140,
      filters: periods,
      onFilter: (value: string, record: any) =>
        periods.find((item: Filter) => item.value === value)?.value ===
        record.period,
    },
    {
      title: "Дата получения",
      dataIndex: "date",
      key: "date",
      ...getColumnSearchProps("date"),
      width: 170,
    },
    {
      title: "Примечание",
      dataIndex: "note",
      key: "note",
      width: 450,
      ...getColumnSearchProps("note"),
    },
    {
      title: "Действия",
      dataIndex: ["operation"],
      width: 100,
      editable: false,
      fixed: "right",
      render: (_: any, record: IUserReportTable) => {
        return (
          <Link to={`${url}/${record.id}`}>
            <EditFilled title={"Редактировать"} style={{ fontSize: "16px" }} />
          </Link>
        );
      },
    },
  ];

  return (
    <Switch>
      <Route exact path={path}>
        <Table
          dataSource={data}
          columns={columns}
          rowKey="id"
          bordered
          loading={loading}
          rowClassName="editable-row correct-row"
          scroll={{ y: 700 }}
          pagination={{ pageSize: 100 }}
        />
      </Route>
      <Route path={`${path}/:id`}>
        <CorrectBalanceSheetPage />
      </Route>
    </Switch>
  );
};

export default CorrectReportPage;
