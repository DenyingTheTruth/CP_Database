import React, { useState, useEffect, useRef, ReactText } from "react";
import {
  Table,
  Popconfirm,
  Form,
  DatePicker,
  Input,
  Space,
  Button,
} from "antd";
import {
  SearchOutlined,
  EditFilled,
  RollbackOutlined,
  SaveOutlined,
} from "@ant-design/icons";
import Highlighter from "react-highlight-words";

import moment, { Moment } from "moment";
import { IIntervals } from "../../../../models/common-interfaces";
import {
  IntervalPeriod,
  Organization,
  ReportType,
} from "../../../../models/common-types";
import { TableRowSelection } from "antd/lib/table/interface";

interface Item {
  key: string;
  name: string;
  [propName: string]: IntervalPeriod | string;
}

interface EditableCellProps extends React.HTMLAttributes<HTMLElement> {
  editing: boolean;
  dataIndex: string[];
  title: any;
  inputType: "number" | "text" | Moment;
  record: Item;
  index: number;
  children: React.ReactNode;
}

const EditableCell: React.FC<EditableCellProps> = ({
  editing,
  dataIndex,
  title,
  inputType,
  record,
  index,
  children,
  ...restProps
}) => {
  // noinspection RequiredAttributes
  const cell = editing ? (
    <Form.Item name={dataIndex} style={{ margin: 0 }}>
      <DatePicker format={"DD.MM.YYYY"} />
    </Form.Item>
  ) : (
    children
  );
  return <td {...restProps}>{cell}</td>;
};

export const ReportingDeadlinesTable = ({
  intervals,
  reportInfo,
  onTableRowSave,
  tableLoading,
  isTableLoading,
  orgForCopyFrom,
  orgsForCopyTo,
  onOrgsForCopyToChange,
  isTableEdit,
}: {
  intervals: Array<IIntervals>;
  reportInfo: ReportType | undefined;
  onTableRowSave: (data: any) => void;
  tableLoading: (value: boolean) => void;
  isTableLoading: boolean;
  orgForCopyFrom: string | undefined;
  orgsForCopyTo: ReactText[];
  onOrgsForCopyToChange: (value: ReactText[]) => void;
  isTableEdit: (value: boolean) => void;
}) => {
  const [form] = Form.useForm();
  const [data, setData] = useState<Array<Item>>([]);
  const [editingKey, setEditingKey] = useState("");
  const [currColumns, setCurrColumns] = useState<any>([]);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<string>("");

  useEffect(() => {
    if (intervals.length > 0 && reportInfo) {
      const dataSource = intervals.map((item, i) => {
        const { periods } = item;
        const periodProps: { [propName: string]: IntervalPeriod } = {};
        periods.forEach((item, index) => {
          item.date = item.date ? moment(item.date) : null;
          periodProps[`period${index}`] = item;
        });
        return {
          key: item.organizationId,
          name: item.organization.name,
          ...periodProps,
        };
      });
      setData(dataSource);

      reportInfo.periods.forEach((item, i) => {
        const prop: string = `period${i}`;
        const newCol = {
          title: item.name,
          dataIndex: [prop, "date"],
          editable: true,
          width: 200,
          // render: (_: any, record: Item) => {
          //   const period = record[prop];
          //   return (
          //     <span>
          //       {period && typeof period !== "string" && period.date
          //         ? moment(period.date).format("DD.MM.YYYY")
          //         : null}
          //     </span>
          //   );
          // },
          ...getColumnSearchProps([prop, "date"]),
        };
        columns.splice(-1, 0, newCol);
      });

      if (orgForCopyFrom) {
        columns.pop();
        columns.push({
          title: "Действия",
          dataIndex: ["operation"],
          width: 90,
          editable: false,
          render: (_: any, record: Item) => {
            return (
              <button
                className={"action-button-link"}
                disabled={true}
                onClick={() => edit(record)}
              >
                <EditFilled
                  title={"Редактировать"}
                  style={{ fontSize: "16px" }}
                />
              </button>
            );
          },
        });
      } else {
        columns.pop();
        columns.push({
          title: "Действия",
          dataIndex: ["operation"],
          width: 90,
          editable: false,
          render: (_: any, record: Item) => {
            const editable = isEditing(record);
            return editable ? (
              <span style={{ display: "flex" }}>
                <button
                  className={"action-button-link"}
                  onClick={() => save(record)}
                  style={{ marginRight: 8 }}
                >
                  <SaveOutlined
                    title={"Сохранить"}
                    style={{ fontSize: "16px" }}
                  />
                </button>
                <Popconfirm
                  title="Уверены что хотите отменить изменения?"
                  placement="left"
                  onConfirm={cancel}
                  okText={"Да"}
                  cancelText={"Нет"}
                >
                  <a>
                    <RollbackOutlined
                      title={"Отмена"}
                      style={{ fontSize: "16px" }}
                    />
                  </a>
                </Popconfirm>
              </span>
            ) : (
              <button
                className={"action-button-link"}
                disabled={editingKey !== ""}
                onClick={() => edit(record)}
              >
                <EditFilled
                  title={"Редактировать"}
                  style={{ fontSize: "16px" }}
                />
              </button>
            );
          },
        });
      }

      const mergedColumns: any = columns.map((col: any) => {
        if (!col.editable) {
          return col;
        }
        return {
          ...col,
          onCell: (record: Item) => ({
            record,
            dataIndex: col.dataIndex,
            title: col.title,
            editing: isEditing(record),
          }),
        };
      });
      setCurrColumns(mergedColumns);
    }
  }, [intervals, reportInfo, editingKey, orgForCopyFrom]);

  useEffect(() => {
    tableLoading(false);
  }, [intervals]);

  const isEditing = (record: Item) => record.key === editingKey;

  const onSelectChange = (
    selectedRowKeys: ReactText[],
    selectedRows: Item[],
  ) => {
    onOrgsForCopyToChange(selectedRowKeys);
  };

  const edit = (record: Item) => {
    form.setFieldsValue({ ...record });
    setEditingKey(record.key);
    isTableEdit(true);
  };

  const cancel = () => {
    setEditingKey("");
    isTableEdit(false);
  };

  const save = async (record: Item) => {
    try {
      const row = (await form.validateFields()) as Item;
      const newData = [...data];
      const index = newData.findIndex((item) => record.key === item.key);
      if (index > -1) {
        const item = newData[index];
        const newItem = JSON.parse(JSON.stringify(item));
        for (let property in row) {
          if (
            newItem.hasOwnProperty(property) &&
            newItem[property].hasOwnProperty("date")
          ) {
            const { date } =
              typeof row[property] !== "string"
                ? row[property]
                : newItem[property];
            newItem[property].date = date
              ? moment(date).format("YYYY-MM-DD")
              : null;
          } else {
            newItem[property] = row[property];
          }
        }
        const isChange = JSON.stringify(item) !== JSON.stringify(newItem);
        setEditingKey("");
        if (isChange) {
          tableLoading(true);
          onTableRowSave(newItem);
        }
      } else {
        setEditingKey("");
      }
    } catch (errInfo) {
      console.log("Validate Failed:", errInfo);
    }
    isTableEdit(false);
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
    }) => {
      return (
        <div
          style={
            dataIndex[1] === "date"
              ? {
                  display: "flex",
                  flexFlow: "column",
                  alignItems: "center",
                  padding: 8,
                }
              : { padding: 8 }
          }
        >
          {dataIndex[1] === "date" ? (
            <DatePicker
              autoFocus={true}
              ref={inputRef}
              onChange={(e) =>
                setSelectedKeys(e ? [e?.format("DD.MM.yyyy")] : [])
              }
              format={"DD.MM.yyyy"}
              style={{ marginBottom: 8 }}
            />
          ) : (
            <Input
              ref={inputRef}
              placeholder={`Поиск`}
              value={selectedKeys[0]}
              onChange={(e) =>
                setSelectedKeys(e.target.value ? [e.target.value] : [])
              }
              onPressEnter={() =>
                handleSearch(selectedKeys, confirm, dataIndex)
              }
              style={{ width: 188, marginBottom: 8, display: "block" }}
            />
          )}
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
      let newRecord = { ...record };
      if (dataIndex !== "name") {
        const length = dataIndex.length;
        for (let i = 0; i < length; i++) {
          newRecord = newRecord[dataIndex[i]];
        }
      } else {
        newRecord = newRecord[dataIndex];
      }
      return newRecord
        ? typeof newRecord === "string"
          ? newRecord.toString().toLowerCase().includes(value.toLowerCase())
          : newRecord
              .format("DD.MM.yyyy")
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
    render: (text: any) => {
      const length = dataIndex.length;
      return searchedColumn[length - 1] === dataIndex[length - 1] ? (
        <Highlighter
          highlightStyle={{ padding: 0 }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text?.toString()}
        />
      ) : typeof text === "string" ? (
        text
      ) : (
        text?.format("DD.MM.yyyy")
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
      title: "Организация",
      dataIndex: ["name"],
      key: "name",
      editable: false,
      sortOrder: "ascend",
      sorter: (a: Organization, b: Organization) => {
        if (a.name < b.name) {
          return -1;
        }
        if (a.name > b.name) {
          return 1;
        }
        return 0;
      },
      ...getColumnSearchProps("name"),
    },
    {
      title: "Действия",
      dataIndex: ["operation"],
      width: 130,
      editable: false,
      render: (_: any, record: Item) => {
        const editable = isEditing(record);
        return editable ? (
          <span style={{ display: "flex" }}>
            <button
              className={"action-button-link"}
              onClick={() => save(record)}
              style={{ marginRight: 8 }}
            >
              Сохранить
            </button>
            <Popconfirm
              title="Уверены что хотите отменить изменения?"
              onConfirm={cancel}
              okText={"Да"}
              cancelText={"Нет"}
            >
              <a>Отмена</a>
            </Popconfirm>
          </span>
        ) : (
          <button
            className={"action-button-link"}
            disabled={editingKey !== ""}
            onClick={() => edit(record)}
          >
            <EditFilled title={"Редактировать"} style={{ fontSize: "16px" }} />
          </button>
        );
      },
    },
  ];

  const rowSelection: TableRowSelection<Item> = !orgForCopyFrom
    ? {
        columnWidth: 0,
        hideSelectAll: true,
        renderCell: () => "",
      }
    : {
        selectedRowKeys: orgsForCopyTo,
        onChange: onSelectChange,
        renderCell: (checked, record, index, originNode) => {
          if (record.key === orgForCopyFrom) {
            return "";
          }
          return originNode;
        },
      };

  return (
    <Form form={form} component={false}>
      <Table
        components={{
          body: {
            cell: EditableCell,
          },
        }}
        bordered
        className={!orgForCopyFrom ? "hide-row-selection" : ""}
        dataSource={data}
        columns={currColumns}
        rowClassName={(record) =>
          record.key === orgForCopyFrom
            ? "editable-row row-copy"
            : "editable-row"
        }
        pagination={false}
        loading={isTableLoading}
        rowSelection={rowSelection}
        scroll={{ x: "max-content", y: "none" }}
      />
    </Form>
  );
};
