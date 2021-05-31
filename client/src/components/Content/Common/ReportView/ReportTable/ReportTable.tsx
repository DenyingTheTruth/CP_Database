import React, {
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import { Form, Input, InputNumber, Table } from "antd";
import { v4 as uuidV4 } from "uuid";
import { openNotification } from "../../../../../helpers/helpers";
import { AuthContext } from "../../../../Auth/Auth";
import "./ReportTable.scss";
const EditableContext = createContext<any>(undefined);

interface EditableRowProps {
  index: number;
}

const EditableRow: React.FC<EditableRowProps> = ({ index, ...props }) => {
  const [form] = Form.useForm();
  return (
    <Form form={form} component={false}>
      <EditableContext.Provider value={form}>
        <tr {...props} />
      </EditableContext.Provider>
    </Form>
  );
};

const handleWheel = (e: any) => e.preventDefault();

interface EditableCellProps {
  title: React.ReactNode;
  editable: boolean;
  children: React.ReactNode;
  dataIndex: string;
  record: any;
  codeItem: string;
  isCheck: boolean;
  isCorrect: boolean;
  readOnlyCells: any[];
  bindings: any[];
  tabIndex: number;
  handleSave: (record: any) => void;
  isNotValidCell: (record: any, dataIndex: string) => boolean;
  handleAddToNotValid: (validObj: any) => void;
}

const EditableCell: React.FC<EditableCellProps> = ({
  title,
  editable,
  children,
  dataIndex,
  record,
  handleSave,
  handleAddToNotValid,
  isCheck,
  isCorrect,
  isNotValidCell,
  readOnlyCells,
  bindings,
  tabIndex,
  ...restProps
}) => {
  const [editing, setEditing] = useState(false);
  const inputRef = useRef<any>(null);
  const form = useContext(EditableContext);

  useEffect(() => {
    if (editing) {
      inputRef?.current?.focus();
      inputRef?.current?.input?.addEventListener("wheel", handleWheel);
    }
    return () => {
      inputRef?.current?.input?.removeEventListener("wheel", handleWheel);
    };
  }, [editing]);

  const toggleEdit = () => {
    setEditing(!editing);
    form.setFieldsValue({ [dataIndex]: record[dataIndex] });
  };

  const toggleCellValid = (e: any) => {
    e.target.classList.toggle("not-valid-cell");
  };

  const addToNotValid = (record: any, e: any) => {
    toggleCellValid(e);
    handleAddToNotValid({ codeItem: record.codeItem, cell: dataIndex });
  };

  const checkReadOnly = () => {
    return readOnlyCells?.find(
      (x: any) => x.codeItem === record.codeItem && x.cell === dataIndex,
    );
  };

  const checkColCalc = () => {
    const colTarget = bindings?.find((x: any) => x.type === "column");
    if (colTarget) {
      return (
        dataIndex === colTarget.target &&
        !colTarget.ignore?.includes(record.codeItem)
      );
    }
    return false;
  };

  const findNextCell = (e: any) => {
    const { activeElement }: { activeElement: Element | null } = document;

    if (activeElement && activeElement.tagName !== "BODY") {
      const currentCell =
        activeElement?.parentNode?.parentNode?.parentNode?.parentNode
          ?.parentNode?.parentNode?.parentNode;

      const currentRow = currentCell?.parentNode as HTMLElement;

      let nextRow =
        e.keyCode === 38
          ? document.querySelector(
              `[id$='panel-${tabIndex}'] [data-row-key="${
                Number(currentRow?.dataset.rowKey) - 1
              }"]`,
            )
          : e.keyCode === 40 || e.keyCode === 13
          ? document.querySelector(
              `[id$='panel-${tabIndex}'] [data-row-key="${
                Number(currentRow?.dataset.rowKey) + 1
              }"]`,
            )
          : document.querySelector(
              `[id$='panel-${tabIndex}'] [data-row-key="${Number(
                currentRow?.dataset.rowKey,
              )}"]`,
            );

      // @ts-ignore
      const focusIndex = [...currentRow?.childNodes].findIndex(
        (x) => x === currentCell,
      );

      const newFocusIndex =
        e.keyCode === 39
          ? focusIndex + 1
          : e.keyCode === 37
          ? focusIndex - 1
          : focusIndex;

      let nextCell;

      if (nextRow || e.keyCode === 38) {
        nextCell = nextRow?.childNodes[newFocusIndex]
          ?.firstChild as HTMLElement;
      } else {
        nextRow = document.querySelector(
          `[id$='panel-${tabIndex}'] [data-row-key="${0}"]`,
        );

        nextCell =
          e.keyCode === 40 || e.keyCode === 13
            ? (nextRow?.childNodes[newFocusIndex + 1]
                ?.firstChild as HTMLElement)
            : (nextRow?.childNodes[newFocusIndex - 1]
                ?.firstChild as HTMLElement);
      }
      return nextCell;
    }
  };

  const save = async (e?: any) => {
    try {
      const nextCell = findNextCell(e);
      const value = await form.validateFields();
      const arrValue = Object.entries(value);
      if (arrValue[0][1] === "" || arrValue[0][1] === 0) {
        arrValue[0][1] = null;
      }
      toggleEdit();
      handleSave({ ...record, ...Object.fromEntries(arrValue) });
      nextCell?.click();
      nextCell?.querySelector("input")?.focus();
    } catch (errInfo) {
      console.log(errInfo);
      if (!errInfo.hasOwnProperty("errorFields")) {
        const type = "error";
        const title = "Ошибка сохранения значения";
        const msg =
          "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
        openNotification(title, msg, type);
      }
    }
  };

  const keyDownHandle = (e: any) => {
    if (
      e.keyCode === 190 ||
      e.keyCode === 191 ||
      e.keyCode === 110 ||
      e.keyCode === 188 ||
      e.keyCode === 107
    )
      e.preventDefault();
    if ((e.keyCode > 36 && e.keyCode < 41) || e.keyCode === 13) {
      e.preventDefault();
      save(e);
    }
  };

  let childNode = <td {...restProps}>{children}</td>;

  if (!editable && !(dataIndex === "2" || dataIndex === "1")) {
    const cellProps =
      isCorrect && isNotValidCell(record, dataIndex)
        ? {
            className: "not-valid-cell",
            style: {
              paddingRight: 24,
              height: "100%",
              width: "100%",
              cursor: "pointer",
            },
          }
        : {};
    childNode = (
      <td {...restProps}>
        <div {...cellProps}>{children}</div>
      </td>
    );
  }

  if (editable && !isCheck && !checkReadOnly() && !checkColCalc()) {
    // noinspection RequiredAttributes
    childNode = editing ? (
      <td
        {...restProps}
        style={{ backgroundColor: "#FAFAFA", fontWeight: 400 }}
      >
        <Form.Item style={{ margin: 0, width: "100%" }} name={dataIndex}>
          <InputNumber
            type="number"
            style={{ width: "100%" }}
            ref={inputRef}
            onBlur={() => save()}
            onFocus={() => inputRef.current.select()}
            onKeyDown={(e) => keyDownHandle(e)}
            keyboard={false}
          />
        </Form.Item>
      </td>
    ) : (
      <td
        {...restProps}
        style={{ backgroundColor: "#FAFAFA", fontWeight: 400 }}
      >
        <div
          className={`editable-cell-value-wrap ${
            isCorrect && isNotValidCell(record, dataIndex)
              ? "not-valid-cell"
              : ""
          }`}
          style={{
            paddingRight: 24,
            height: "100%",
            width: "100%",
            cursor: "pointer",
          }}
          onClick={toggleEdit}
        >
          {children}
        </div>
      </td>
    );
  }

  if (isCheck) {
    childNode = (
      <td {...restProps}>
        <div
          className={`editable-cell-value-wrap ${
            isNotValidCell(record, dataIndex) ? "not-valid-cell" : ""
          }`}
          style={{
            paddingRight: 24,
            height: "100%",
            width: "100%",
            cursor: "pointer",
          }}
          onClick={(e) => addToNotValid(record, e)}
        >
          {children}
        </div>
      </td>
    );
  }

  return childNode;
};

const getColumnWidth = (index: number) => {
  if (index === 0) return 340;

  if (index === 1) return 80;

  return "auto";
};

const ReportTable = ({
  tabIndex,
  tabData,
  setData,
  setAdditionalData,
  setNotValid,
  isCheck,
  isVault,
  isCorrect,
}: {
  tabIndex: number;
  tabData: any;
  setData: (tab: number, row: any) => void;
  setNotValid: (tab: number, validObj: any) => void;
  setAdditionalData: (tab: number, dataObj: any, key: string) => void;
  isCheck: boolean;
  isVault: boolean;
  isCorrect: boolean;
}) => {
  const [footerForm] = Form.useForm();
  let counter: number = 0;

  const isAdmin = useContext(AuthContext).user?.role;

  useEffect(() => {
    if (tabData && isAdmin === "Admin") {
      footerForm.setFieldsValue({ ...tabData.footer });
    }
  }, []);

  const handleSave = (row: any) => {
    setData(tabIndex, row);
  };

  const handleAddToNotValid = (validObj: any) => {
    setNotValid(tabIndex, validObj);
  };

  const handleSaveAdditional = (dataObj: any, key: string) => {
    setAdditionalData(tabIndex, dataObj, key);
  };

  const saveAdditional = async (targetForm: any, key: string) => {
    try {
      const value = await targetForm.validateFields();
      handleSaveAdditional({ ...tabData[key], ...value }, key);
    } catch (errInfo) {
      const type = "error";
      const title = "Ошибка сохранения значения";
      const msg =
        "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
  };

  const isEditableRow = (item: any) => {
    if (isVault) return false;
    const isCalc = tabData?.bindings?.find(
      (bind: any) => bind.type === "row" && bind.target === item.codeItem,
    );
    return !isCalc;
  };

  const isEditableCol = (item: any, index: number) => {
    if (isVault) return false;
    const isCalc = tabData?.bindings?.find(
      (bind: any) => bind.type === "column" && bind.target === item.dataIndex,
    );
    return !isCalc && index > 1;
  };

  const isNotValidCell = (obj: any, dataIndex: string) => {
    const cell = tabData?.validations?.find(
      (item: any) => item.codeItem === obj.codeItem && dataIndex === item.cell,
    );
    return !!cell;
  };

  const bracketsView = (value: any, row: any, dataIndex: string) => {
    if (value === null) return "-";

    const isNegative = value < 0;
    let isBracketsCol = tabData?.subtractedRows?.includes(row.codeItem);

    //нет вычитаемых строк и столбцов
    if (!isBracketsCol && tabData?.subtractedColumns === null) {
      return isNegative ? `(${Math.abs(value)})` : `${Math.abs(value)}`;
    }

    if (!isBracketsCol && tabData?.subtractedColumns) {
      const keys = Object.keys(row);
      isBracketsCol = keys.some(
        (key) => tabData?.subtractedColumns.includes(key) && key === dataIndex,
      );
    }

    if (isBracketsCol !== isNegative) return `(${Math.abs(value)})`;

    return `${Math.abs(value)}`;
  };

  const preProcessedColumns = (columns: any) => {
    const result = columns?.map((item: any, index: number) => {
      const { title, dataIndex } = item;
      return {
        title,
        children: [
          {
            title: index + 1,
            editable: isEditableCol(item, index),
            className:
              !isEditableCol(item, index) && index > 2 ? "cell-calc-value" : "",
            width: getColumnWidth(index),
            dataIndex,
            render: (value: any, row: any) => {
              switch (item.dataIndex) {
                case "title":
                  return row.titlePosition || !row.codeItem
                    ? {
                        children: <span>{value}</span>,
                        props: {
                          colSpan: columns.length,
                          style: { textAlign: "left" },
                        },
                      }
                    : {
                        children: <span>{value}</span>,
                        props: { style: { textAlign: "left" } },
                      };
                case "codeItem":
                  return row.titlePosition || !row.codeItem
                    ? {
                        children: <span>{value}</span>,
                        props: {
                          colSpan: 0,
                          style: { textAlign: "center", width: "100px" },
                        },
                      }
                    : {
                        children: <span>{value}</span>,
                        props: {
                          style: { textAlign: "center", width: "100px" },
                        },
                      };
                default:
                  return row.titlePosition || !row.codeItem
                    ? {
                        children: <span>{value}</span>,
                        props: {
                          colSpan: 0,
                        },
                      }
                    : {
                        children: (
                          <span>
                            {bracketsView(value, row, item.dataIndex)}
                          </span>
                        ),
                        props: {
                          style: { textAlign: "right" },
                        },
                      };
              }
            },
          },
        ],
      };
    });

    return result?.map((col: any, index: number) => {
      if (index < 2) {
        return col;
      }
      return {
        ...col,
        children: [
          {
            ...col.children[0],
            onCell: (record: any) => ({
              record,
              className: `${isEditableRow(record) ? "" : "cell-calc-value"}`,
              editable: isEditableRow(record),
              dataIndex: col.children[0].dataIndex,
              title: col.children[0].title,
              isCheck,
              isCorrect,
              tabIndex,
              readOnlyCells: tabData?.readOnlyCells,
              bindings: tabData?.bindings,
              handleSave: handleSave,
              handleAddToNotValid: handleAddToNotValid,
              isNotValidCell: isNotValidCell,
            }),
          },
        ],
      };
    });
  };

  // noinspection RequiredAttributes
  return (
    <Table
      rowClassName={(record) => {
        if (record.titlePosition) {
          return "title-row";
        }
        if (record.codeParent) {
          return "child-row editable-row";
        }
        if (record.codeItem) {
          return "header-row editable-row";
        }
        return "editable-row";
      }}
      bordered
      pagination={false}
      title={() => (
        <>
          <div className={"table-header-attachment"}>
            <div>
              <div>
                {tabData?.attachment?.title ?? tabData?.attachment?.Title}
              </div>
              <div
                dangerouslySetInnerHTML={{
                  __html:
                    tabData?.attachment?.text ?? tabData?.attachment?.Text,
                }}
              />
            </div>
          </div>
          <h2 className={"table-header-title"}>
            <span>{tabData?.title}</span>
            <br />
            <span>{tabData?.titleDate}</span>
          </h2>
        </>
      )}
      footer={() => {
        if (isAdmin === "Admin")
          return (
            <Form form={footerForm}>
              <div className={"table-footer-row"}>
                <Form.Item name={"leaderName"}>
                  <Input
                    onPressEnter={() => saveAdditional(footerForm, "footer")}
                    onBlur={() => saveAdditional(footerForm, "footer")}
                    readOnly={isCheck}
                  />
                </Form.Item>
                <span />
                <Form.Item name={"leader"}>
                  <Input
                    onPressEnter={() => saveAdditional(footerForm, "footer")}
                    onBlur={() => saveAdditional(footerForm, "footer")}
                    readOnly={isCheck}
                  />
                </Form.Item>
              </div>
              <div className={"table-footer-row"}>
                <Form.Item name={"accountantGeneral"}>
                  <Input
                    onPressEnter={() => saveAdditional(footerForm, "footer")}
                    onBlur={() => saveAdditional(footerForm, "footer")}
                    readOnly={isCheck}
                  />
                </Form.Item>
                <span />
                <Form.Item name={"chiefAccountant"}>
                  <Input
                    onPressEnter={() => saveAdditional(footerForm, "footer")}
                    onBlur={() => saveAdditional(footerForm, "footer")}
                    readOnly={isCheck}
                  />
                </Form.Item>
              </div>
            </Form>
          );
      }}
      dataSource={tabData?.table?.rows.map((r: any) => {
        if (
          r.codeItem &&
          !tabData?.bindings?.find((x: any) => x.target === r.codeItem)
        ) {
          return { ...r, key: counter++ };
        }
        return { ...r, key: uuidV4() };
      })}
      components={{
        body: {
          row: EditableRow,
          cell: EditableCell,
        },
      }}
      columns={preProcessedColumns(tabData?.table?.columns)}
      scroll={{ x: 960, y: "none" }}
      className={"table-create"}
    />
  );
};

export default ReportTable;