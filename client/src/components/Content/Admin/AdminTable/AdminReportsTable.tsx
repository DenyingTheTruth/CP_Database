import React, { useEffect, useState, useRef } from "react";
import { Link } from "react-router-dom";
import { Table, Input, Button, Space, DatePicker } from "antd";
import Highlighter from "react-highlight-words";
import {
  SearchOutlined,
  FormOutlined,
  FileWordOutlined,
} from "@ant-design/icons";
import moment from "moment";

import {
  getSentReports,
  downloadFile,
} from "../../../../services/report/report.services";

import { statusReportAdmin } from "../../../../constants/constants";

import { Filter } from "../../../../models/common-types";

const AdminHomeTable = () => {
  const [data, setData] = useState([]);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<Array<string>>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [years, setYears] = useState<Array<Filter>>([]);
  const [periods, setPeriods] = useState<Array<Filter>>([]);

  useEffect(() => {
    setLoading(true);
    (async function () {
      const newData = await getSentReports();
      newData.map((item: any) => {
        item.date = moment(item.date).format("DD.MM.YYYY");
        item.replyDate = item.replyDate
          ? moment(item.replyDate).format("DD.MM.YYYY")
          : "-";
      });
      const filterYears: Array<Filter> = [];
      const filterPeriods: Array<Filter> = [];
      const newYears: Array<number> = [];
      const newPeriods: Array<string> = [];
      newData.forEach((item: any) => {
        newYears.push(item.userCheckinInterval.year);
      });
      newData.forEach((item: any) => {
        newPeriods.push(item.userCheckinInterval.period.name);
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

  const getColumnSearchProps = (dataIndex: any) => ({
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
    }) => {
      return dataIndex[dataIndex.length - 1] === "name" ? (
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
      ) : (
        <div
          style={{
            display: "flex",
            flexFlow: "column",
            alignItems: "center",
            padding: 8,
          }}
        >
          <DatePicker
            autoFocus={true}
            ref={inputRef}
            onChange={(e) =>
              setSelectedKeys(e ? [e?.format("DD.MM.yyyy")] : [])
            }
            format={"DD.MM.yyyy"}
            style={{ marginBottom: 8 }}
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
      );
    },
    filterIcon: (filtered: any) => (
      <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
    ),
    onFilter: (value: any, record: any) => {
      const length = dataIndex.length;
      let newRecord = { ...record };
      for (let i = 0; i < length; i++) {
        newRecord = newRecord[dataIndex[i]];
      }
      return newRecord
        ? newRecord.toString().toLowerCase().includes(value.toLowerCase())
        : "";
    },
    onFilterDropdownVisibleChange: (visible: any) => {
      if (visible) {
        setTimeout(() => inputRef.current.focus(), 100);
      }
    },
    render: (text: any) => {
      const length = dataIndex.length;
      return searchedColumn[length - 1] === dataIndex[length - 1] ? (
        <Highlighter
          highlightStyle={{ backgroundColor: "#ffc069", padding: 0 }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text?.toString()}
        />
      ) : (
        text
      );
    },
  });

  const handleSearch = (
    selectedKeys: any[],
    confirm: any,
    dataIndex: Array<string>,
  ) => {
    confirm();
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex);
  };

  const handleReset = (clearFilters: () => void) => {
    clearFilters();
    setSearchText("");
  };

  const columns: any = [
    {
      title: "Название организации",
      dataIndex: ["userCheckinInterval", "organization", "name"],
      key: "orgName",
      width: 300,
      ...getColumnSearchProps(["userCheckinInterval", "organization", "name"]),
    },
    {
      title: "Название отчета",
      dataIndex: ["reportType", "name"],
      key: "reportName",
      width: 350,
      ...getColumnSearchProps(["reportType", "name"]),
    },
    {
      title: "Год",
      filters: years,
      dataIndex: ["userCheckinInterval", "year"],
      className: "ant-col-custom-size",
      width: 90,
      onFilter: (value: string, record: any) =>
        years.find((item: Filter) => item.value === value)?.value ===
        record.userCheckinInterval.year.toString(),
    },
    {
      title: "Отчетный период",
      dataIndex: ["userCheckinInterval", "period", "name"],
      key: "userCheckinInterval",
      filters: periods,
      onFilter: (value: string, record: any) =>
        periods.find((item: Filter) => item.value === value)?.value ===
        record.userCheckinInterval.period.name,
      width: 140,
    },
    {
      title: "Дата отправки организацией",
      dataIndex: ["date"],
      key: "reportDate",
      ...getColumnSearchProps(["date"]),
      width: 170,
    },
    {
      title: "Приложение",
      dataIndex: ["attachmentFile"],
      key: "attachmentFile",
      width: 125,
      render: (value: any, record: any) => {
        return (
          <div className="flex-row --row-center">
            {value ? (
              <Button
                type="text"
                className="action-button-link"
                onClick={async () => await downloadFile(record.id, value.name)}
              >
                <FileWordOutlined
                  style={{ fontSize: "28px", color: "#295394" }}
                />
              </Button>
            ) : (
              ""
            )}
          </div>
        );
      },
    },
    {
      title: "Статус",
      dataIndex: ["adminStatusReport"],
      key: "adminStatusReport",
      filters: [
        { text: "Новый", value: "Новый" },
        {
          text: "Отправлен на корректировку",
          value: "Отправлен на корректировку",
        },
        { text: "После корректировки", value: "После корректировки" },
      ],
      render: (_: any, record: any) => {
        const status = statusReportAdmin[record.adminStatusReport].text;
        return <span>{status}</span>;
      },
      onFilter: (value: string, record: any) => {
        const filter: any = statusReportAdmin.find(
          (item: any) => item.value === value,
        );
        return record.adminStatusReport === statusReportAdmin.indexOf(filter);
      },
      width: 200,
    },
    {
      title: "Дата ответа (дата приемки)",
      dataIndex: ["replyDate"],
      key: "replyDate",
      ...getColumnSearchProps(["replyDate"]),
      width: 150,
    },
    {
      title: "Действия",
      dataIndex: ["operation"],
      width: 100,
      editable: false,
      fixed: "right",
      render: (_: any, record: any) => {
        return record.adminStatusReport !== 1 ? (
          <Link
            to={`/check-report/${record.id}?year=${record.userCheckinInterval.year}`}
          >
            <FormOutlined
              title={"Редактировать"}
              style={{ fontSize: "18px" }}
            />
          </Link>
        ) : (
          <FormOutlined
            title={"Редактировать"}
            style={{ fontSize: "18px", color: "gray" }}
          />
        );
      },
    },
  ];

  return (
    <Table
      dataSource={data}
      columns={columns}
      rowKey="id"
      bordered
      loading={loading}
      rowClassName={(row: any) =>
        `editable-row ${
          !row.isRead &&
          (row.adminStatusReport === 0 || row.adminStatusReport === 2)
            ? "row-unread"
            : ""
        }`
      }
      scroll={{ y: "none" }}
      pagination={{ pageSize: 100 }}
    />
  );
};

export default AdminHomeTable;
