import React, { useState, useEffect, useRef } from "react";
import { Table, Button, Input, Space, Popover } from "antd";
import { ExportOutlined, SearchOutlined } from "@ant-design/icons";
import moment from "moment";
import Highlighter from "react-highlight-words";

import {
  exportById,
  getUserReportLogs,
} from "../../services/report/report.services";

import { statusReportUser } from "../../constants/constants";
import { IUserReportHistory } from "../../models/common-interfaces";
import { Filter } from "../../models/common-types";
import { openNotification } from "../../helpers/helpers";

const HistoryReportPage = () => {
  const [data, setData] = useState<Array<any>>([]);
  const [years, setYears] = useState<Array<Filter>>([]);
  const [periods, setPeriods] = useState<Array<Filter>>([]);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    setLoading(true);
    (async function () {
      const getData = await getUserReportLogs();
      const newData: Array<IUserReportHistory> = [];
      getData.logs.forEach((item: any, i: number) => {
        newData.push({
          index: i + 1,
          id: item.id,
          name: item.report.reportType.name,
          status: statusReportUser[item.statusReport].text,
          note: item.note,
          date: item.date ? moment(item.date).format("DD.MM.YYYY") : "-",
          year: item.report.userCheckinInterval.year,
          period: item.report.userCheckinInterval.period.name,
          reportId: item.report.id,
        });
      });
      const filterYears: Array<Filter> = [];
      const filterPeriods: Array<Filter> = [];
      getData.years.forEach((item: number) => {
        filterYears.push({
          text: item.toString(),
          value: item.toString(),
        });
      });
      getData.periods.forEach((item: string) => {
        filterPeriods.push({
          text: item,
          value: item,
        });
      });
      setData(newData);
      setYears(filterYears);
      setPeriods(filterPeriods);
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

  const exportHandler = async (id: string) => {
    setLoading(true);

    const res = await exportById(id);
    if (res.isSuccess) {
      const type = "success";
      const title = "Экспорт документа выполнен успешно";
      const msg = "";
      openNotification(title, msg, type);
    } else {
      const type = "error";
      const title = "Ошибка экспорта отчета";
      const msg =
        "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    console.log("res", res);
    setLoading(false);
  };
  const columns: any = [
    {
      title: "№",
      dataIndex: "index",
      key: "index",
      render: (text: any, record: any, index: any) => (
        <div style={{ textAlign: "center" }}>{index + 1}</div>
      ),
      width: 70,
    },
    {
      title: "Название отчета",
      dataIndex: "name",
      key: "name",
      ...getColumnSearchProps("name"),
      width: 400,
    },
    {
      title: "Год",
      dataIndex: "year",
      key: "year",
      filters: years,
      onFilter: (value: string, record: any) =>
        years.find((item: Filter) => item.value === value)?.value ===
        record.year.toString(),
      width: 90,
    },
    {
      title: "Период сдачи отчетности",
      dataIndex: "period",
      key: "period",
      filters: periods,
      onFilter: (value: string, record: any) =>
        periods.find((item: Filter) => item.value === value)?.value ===
        record.period,
      width: 140,
    },
    {
      title: "Дата изменения",
      dataIndex: "date",
      key: "date",
      ...getColumnSearchProps("date"),
      width: 170,
    },
    {
      title: "Статус",
      dataIndex: "status",
      key: "status",
      filters: statusReportUser,
      onFilter: (value: string, record: any) => {
        return (
          statusReportUser.find((item: Filter) => item.value === value)
            ?.value === record.status
        );
      },
      width: 200,
    },
    {
      title: "Примечание",
      dataIndex: "note",
      key: "note",
      ...getColumnSearchProps("note"),
    },
    {
      title: "Действия",
      dataIndex: "actions",
      key: "actions",
      render: (text: any, record: any) => {
        const id = record.reportId;
        const style =
          record.status === "Принят" ? `accepted export-img` : `export-img`;
        return record?.status === "Новый" ? (
          <div className="export-img">
            <ExportOutlined style={{ fontSize: "18px", color: "grey" }} />
          </div>
        ) : (
          <div className={style}>
            <Popover content="Экспорт файла Excel">
              <ExportOutlined
                style={{ fontSize: "18px" }}
                onClick={() => exportHandler(id)}
              />
            </Popover>
          </div>
        );

        // return (
        //   <div className={style}>
        //     <Popover content="Экспорт файла Excel">
        //       <ExportOutlined style={{ fontSize: "18px" }} />
        //     </Popover>
        //   </div>
        // );
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
      rowClassName="editable-row"
      pagination={{ pageSize: 100 }}
    />
  );
};

export default HistoryReportPage;
