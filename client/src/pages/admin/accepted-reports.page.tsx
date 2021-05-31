import React, { useState, useEffect, useRef } from "react";
import {
  Table,
  DatePicker,
  Select,
  Popover,
  Button,
  Checkbox,
  Input,
  Space,
} from "antd";
import {
  CheckSquareOutlined,
  CloseSquareOutlined,
  FileWordOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import { Moment } from "moment";
import Highlighter from "react-highlight-words";

import { getReportTypes } from "../../services/directories/directories.services";
import {
  getAcceptedReports,
  sendReportsToCorrection,
  downloadFile,
  exportById,
} from "../../services/report/report.services";
import { ReportType } from "../../models/common-types";
import { useAuth } from "../../components/Auth/Auth";

const { Option } = Select;

const AcceptedReportsPage = () => {
  const { gYear } = useAuth();

  const [data, setData] = useState<Array<any>>([]);
  const [reportTypes, setReportTypes] = useState<Array<ReportType>>([]);
  const [currYear, setCurrYear] = useState<Moment | null>(gYear);
  const [currReportType, setCurrReportType] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [columns, setColumns] = useState<Array<any>>([]);
  const [reportsToCorrection, setReportsToCorrection] = useState<any>([]);
  const [sendClick, setSendClick] = useState<boolean>(true);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<Array<string>>([]);

  useEffect(() => {
    (async function () {
      const newReportTypes = await getReportTypes();
      setReportTypes(newReportTypes);
    })();
  }, []);

  useEffect(() => {
    if (currReportType && currYear)
      (async function () {
        setLoading(true);
        const newColumns = [];
        const reportPeriods = await getReportTypes(currReportType);
        const newData = await getAcceptedReports(
          currYear.year(),
          currReportType,
        );
        setData(newData);

        newColumns.push({
          title: "Название организации",
          dataIndex: ["organization", "name"],
          width: 350,
          ...getColumnSearchProps(["organization", "name"]),
        });

        reportPeriods.periods.forEach((item: any, i: number) => {
          newColumns.push({
            title: item.name,
            filters: [
              { value: "accepted", text: "Сдан" },
              { value: "not accepted", text: "Не сдан" },
            ],
            onFilter: (value: any, record: any) => {
              if (value === "accepted") {
                return record.periods[i].report?.adminStatusReport === 3;
              } else {
                return record.periods[i].report?.adminStatusReport !== 3;
              }
            },
            children: [
              {
                title: "Отчет",
                align: "center",
                dataIndex: ["periods", `${i}`, "report", "adminStatusReport"],
                render: (value: any, record: any) => {
                  return (
                    <div className="flex-row --row-center">
                      {value === 3 ? (
                        <Button
                          type="text"
                          className="action-button-link"
                          onClick={async () => {
                            setLoading(true);
                            await exportById(record.periods[i].report.id);
                            setLoading(false);
                          }}
                        >
                          <CheckSquareOutlined
                            style={{ fontSize: "30px", color: "green" }}
                          />
                        </Button>
                      ) : (
                        <CloseSquareOutlined style={{ fontSize: "30px" }} />
                      )}
                    </div>
                  );
                },
                width: 110,
              },
              {
                title: "Приложение",
                align: "center",
                dataIndex: ["periods", `${i}`, "report", "attachmentFile"],
                render: (value: any, record: any) => {
                  return (
                    <div className="flex-row --row-center">
                      {record.periods[i].report?.adminStatusReport === 3 &&
                      value ? (
                        <Button
                          type="text"
                          className="action-button-link"
                          onClick={async () =>
                            await downloadFile(
                              record.periods[i].report.id,
                              value.name,
                            )
                          }
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
                width: 120,
              },
            ],
          });
        });

        newColumns.push({
          title: "Действия",
          render: (record: any) => {
            const newArr = record.periods
              .filter((item: any) => {
                return item.report !== null && item.report.statusReport === 2;
              })
              .map((item: any) => {
                return { label: item.name, value: item.report.id };
              });
            const content = (
              <div className="flex-row --flex-col">
                <Checkbox.Group
                  options={newArr}
                  onChange={onCheckboxGroupChange}
                  style={{ marginBottom: "20px" }}
                  className="flex-row --flex-col"
                />
                <Button onClick={returnReports}>Отправить</Button>
              </div>
            );
            return newArr.length > 0 ? (
              <Popover content={content} trigger="click">
                <Button
                  className="action-button-link"
                  disabled={newArr.length <= 0}
                  type="text"
                >
                  Отозвать
                </Button>
              </Popover>
            ) : (
              <Button
                className="action-button-link"
                disabled={true}
                type="text"
              >
                Отозвать
              </Button>
            );
          },
          width: 250,
        });
        setColumns([]);
        setColumns(newColumns);
        setLoading(false);
      })();
  }, [currReportType, currYear, sendClick]);

  useEffect(() => {
    if (sendClick) {
      sendReportsToCorrection(reportsToCorrection);
    }
    setSendClick(false);
  }, [sendClick]);

  const onCheckboxGroupChange = (changedValues: any) => {
    setReportsToCorrection(changedValues);
  };

  const returnReports = () => {
    setSendClick(true);
  };

  const onReportTypeChange = (value: string) => {
    setCurrReportType(value);
  };

  const onYearChange = (value: Moment | null) => {
    setCurrYear(value);
  };

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

  return (
    <>
      <div className="deadlines__grid-row">
        <div className="flex-row --row-start gap-20 width-100">
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
          <DatePicker value={currYear} picker="year" onChange={onYearChange} />
        </div>
      </div>
      <Table
        rowKey="organizationId"
        dataSource={data}
        columns={columns}
        rowClassName="editable-row"
        loading={loading}
        bordered
        scroll={{ y: "none" }}
        pagination={{ pageSize: 100 }}
      />
    </>
  );
};

export default AcceptedReportsPage;
