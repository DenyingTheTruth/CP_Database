import React, { useEffect, useState } from "react";

import {
  Badge,
  Button,
  DatePicker,
  Drawer,
  Empty,
  Form,
  Input,
  Layout,
  Result,
  Spin,
  Tabs,
} from "antd";
import {
  FileDoneOutlined,
  WarningOutlined,
  ExportOutlined,
} from "@ant-design/icons";
import { useHistory, Prompt } from "react-router-dom";

import TopControlPanel from "./TopControlPanel/TopControlPanel";
import ReportTable from "./ReportTable/ReportTable";
import {
  exportReport,
  getFreeReport,
  getReportTab,
  getReportTabs,
  importReportByQuarter,
  saveReport,
  sendReport,
  getReportById,
  getReportByInterval,
  acceptReport,
  sendToCorrectReport,
  exportById,
  getCountCorrectReports,
  getCountUnreadReports,
  getConcernVault,
  validateReport,
  readReport,
  getDataByYearPeriodReportId,
} from "../../../../services/report/report.services";
import moment, { Moment } from "moment";
import { Period } from "../../../../models/common-types";
import "./report-view.scss";
import {
  fullCopyObject,
  openNotification,
  toBase64,
} from "../../../../helpers/helpers";
import { useAuth } from "../../../Auth/Auth";
import { getGeneralInfo } from "../../../../services/settings/organizations.services";

const initRepType = {
  id: "a0ac1e8a-1e3c-448d-b364-1d484271f27f",
  name: "Бухгалтерский баланс и приложения к нему",
};

const initTabsData: any = {
  1: null,
  2: null,
  3: null,
  4: null,
  5: null,
  6: null,
};

const ReportView = ({
  reportId = "",
  correct = false,
  check = false,
  year = "",
  vault = false,
  vaultReq,
  vaultData,
  toggleApply,
}: {
  reportId?: string;
  correct?: boolean;
  check?: boolean;
  year?: string;
  vault?: boolean;
  vaultReq?: boolean;
  vaultData?: any;
  toggleApply?: (value: boolean) => void;
}) => {
  const { changeReportsRevision, changeUnreadReports, gYear, user } = useAuth();
  const idOrganization = user?.organization?.id;

  const [tabs, setTabs] = useState<any>([]);
  const [currYear, setCurrYear] = useState<Moment | null>(
    year ? moment(new Date(year)) : gYear,
  );
  const [currPeriod, setCurrPeriod] = useState<string | undefined>();
  const [existReport, setExistReport] = useState<any>();
  const [periods, setPeriods] = useState<Array<Period>>([]);
  const [tabsData, setTabsData] = useState<any>(initTabsData);
  const [loaderTable, setLoaderTable] = useState<boolean>(false);
  const [loaderPage, setLoaderPage] = useState<boolean>(false);
  const [currFile, setCurrFile] = useState<any>();
  const [block, setBlock] = useState<boolean>(false);
  const [flagNotValid, setFlagNotValid] = useState<number>(0);
  const [visible, setVisible] = useState<boolean>(false);
  const [validError, setValidError] = useState<Array<string>>([]);
  const [activeTab, setActiveTab] = useState<string>("1");
  const [endOfLastYear, setEndOfLastYear] = useState<any>();
  const [samePeriodLastYear, setSamePeriodLastYear] = useState<any>();

  const [note, setNote] = useState<string | undefined>();

  const history = useHistory();

  const [headerForm] = Form.useForm();
  const [additionalForm] = Form.useForm();

  useEffect(() => {
    if (tabsData[1]) {
      headerForm.setFieldsValue({ ...tabsData[1].header });
      const additionalData = Object.entries(
        tabsData[1].additionTable,
      ).map((item: any) => [item[0], item[1] ? moment(item[1]) : null]);
      additionalForm.setFieldsValue({
        ...Object.fromEntries(additionalData),
      });
    }
  }, [tabsData]);

  useEffect(() => {
    const abortController = new AbortController();
    const { signal } = abortController;
    //существование отчёта
    if (reportId) {
      setLoaderPage(true);
      (async function () {
        const report = await getReportById(reportId, signal);
        if (report) {
          if (user?.role === "Admin") {
            await readReport(reportId);
            const count = await getCountUnreadReports();
            changeUnreadReports(count);
          }
          const resultTabs = tabImportConverter(report.tabModels, {
            ...initTabsData,
          });
          const newTabs = await getReportTabs(
            initRepType.name,
            undefined,
            signal,
          );
          if (report.report.attachmentFile?.id) {
            const fileObj = {
              uid: report.report.attachmentFile.id,
              name: report.report.attachmentFile.name || "report-document.docx",
              status: "done",
              url: report.report.id,
            };
            setCurrFile(fileObj);
          } else {
            setCurrFile(undefined);
          }
          setCurrPeriod(report.report.userCheckinInterval.id);
          setExistReport(report);
          setTabs(newTabs.names);
          setTabsData(resultTabs);
        }
        setLoaderPage(false);
      })();
      //  своды
    } else if (typeof vaultReq === "boolean" && vaultData) {
      setLoaderPage(true);
      (async function () {
        const report = await getConcernVault(vaultData);
        if (report.report && report.tabModels) {
          const resultTabs = tabImportConverter(report.tabModels, {
            ...initTabsData,
          });
          const newTabs = await getReportTabs(
            initRepType.name,
            undefined,
            signal,
          );
          setCurrPeriod(report.report.userCheckinInterval.id);
          setTabs(newTabs.names);
          setTabsData(resultTabs);
          setExistReport(report);
        } else {
          setExistReport(null);
        }
        if (toggleApply) {
          toggleApply(false);
        }
        setLoaderPage(false);
      })();
      //  корректирование отчёта
    } else if (!vault) {
      setLoaderPage(true);
      (async function () {
        const newReportTypes = await getFreeReport(
          currYear ? currYear.year() : moment(new Date()).year(),
          initRepType.id,
          signal,
        );
        const freePeriods = newReportTypes.periods?.filter(
          (item: Period) => item.isFree,
        );
        setPeriods(newReportTypes.periods);
        setCurrPeriod(
          freePeriods ? freePeriods[0]?.userCheckinIntervalId : freePeriods,
        );
        setValidError([]);
        if (freePeriods && !freePeriods[0]?.userCheckinIntervalId) {
          setLoaderPage(false);
        }
      })();
    }
    return () => {
      abortController.abort();
    };
  }, [currYear, vaultReq]);

  const currPeriodId = periods.find(
    (el) => el?.userCheckinIntervalId === currPeriod,
  );

  const getDataForThePreviousPeriod = async (period: string) => {
    if (currYear && period && initRepType && idOrganization) {
      const informationOnTheReport = await getDataByYearPeriodReportId(
        currYear?.year() - 1,
        period,
        initRepType.id,
        idOrganization,
      );

      if (informationOnTheReport.isSuccess) {
        const data = await getReportById(informationOnTheReport.value.id);

        return data;
      }
    }
  };

  useEffect(() => {
    const abortController = new AbortController();
    const { signal } = abortController;
    if (currPeriod && !reportId && !vault) {
      setLoaderPage(true);
      (async function () {
        const newTabs = await getReportTabs(
          initRepType.name,
          undefined,
          signal,
        );
        const newTab = await getReportTab(
          initRepType.name,
          1,
          currPeriod,
          signal,
        );

        const saveReportRequest = await getReportByInterval(currPeriod, signal);

        if (saveReportRequest?.isSuccess === false) {
          if (
            currPeriodId?.id &&
            currPeriodId?.id !== "c7c5214f-bdf2-4d6a-bcb7-d027a273dc34"
          ) {
            const data = await getDataForThePreviousPeriod(currPeriodId?.id);
            setSamePeriodLastYear(data?.tabModels);
          }

          const dataEndOfLastYear = await getDataForThePreviousPeriod(
            "c7c5214f-bdf2-4d6a-bcb7-d027a273dc34",
          );
          setEndOfLastYear(dataEndOfLastYear?.tabModels);

          if (currPeriodId?.id === "c7c5214f-bdf2-4d6a-bcb7-d027a273dc34") {
            setSamePeriodLastYear(dataEndOfLastYear?.tabModels);
          }
          if (dataEndOfLastYear) {
            const data = dataEndOfLastYear.tabModels.filter(
              (el: any) => el.tabId === 1,
            )[0];

            if (data) {
              const modifiedDataEndOfLastYear = newTab;
              modifiedDataEndOfLastYear.table.rows.map((el: any, i: number) => {
                return (el.value2 = data.table.rows[i].value1);
              });
              tabsData[1] = modifiedDataEndOfLastYear;
            } else {
              tabsData[1] = newTab;
            }
          } else {
            tabsData[1] = newTab;
          }

          setCurrFile(undefined);
          setTabsData(fullCopyObject(tabsData));
        } else {
          const resultTabs = tabImportConverter(saveReportRequest?.tabModels, {
            ...initTabsData,
          });
          if (saveReportRequest?.report.attachmentFile?.id) {
            const fileObj = {
              uid: saveReportRequest.report.attachmentFile.id,
              name:
                saveReportRequest.report.attachmentFile.name ||
                "report-document.docx",
              status: "done",
              url: saveReportRequest.report.id,
            };
            setCurrFile(fileObj);
          } else {
            setCurrFile(undefined);
          }
          setTabsData(resultTabs);
        }
        setTabs(newTabs.names);
        setValidError([]);
        setExistReport(saveReportRequest);
        setLoaderPage(false);
      })();
    }
    return () => {
      abortController.abort();
    };
  }, [currPeriod]);

  const showDrawer = () => {
    setVisible(true);
    tryCheckReport();
  };
  const onClose = () => {
    setVisible(false);
  };

  const onYearChange = (value: Moment | null) => {
    setCurrYear(value);
    setTabsData(initTabsData);
  };

  const onPeriodChange = (value: string) => {
    setCurrPeriod(value);
    setTabsData(initTabsData);
  };

  const onNoteChange = (value: string) => {
    setNote(value);
  };

  const onUploadFileChange = (file: any) => {
    setCurrFile(file);
  };

  const loadTabData = async (key: string) => {
    setActiveTab(key);
    if (tabsData[Number(key)] === null) {
      setLoaderTable(true);
      tabsData[Number(key)] = await getReportTab(
        initRepType.name,
        Number(key),
        currPeriod,
      );

      if (
        (samePeriodLastYear && Number(key) === 2) ||
        (samePeriodLastYear && Number(key) === 4)
      ) {
        const data = samePeriodLastYear.filter(
          (el: any) => el.tabId === Number(key),
        )[0];
        if (data) {
          const modifiedDataEndOfLastYear = tabsData[Number(key)];

          modifiedDataEndOfLastYear?.table.rows.map((el: any, i: number) => {
            return (el.value2 = data.table.rows[i].value1);
          });
          tabsData[Number(key)] = modifiedDataEndOfLastYear;
        }
      }
      if (endOfLastYear && Number(key) === 6) {
        const data = endOfLastYear.filter(
          (el: any) => el.tabId === Number(key),
        )[0];
        if (data) {
          const modifiedDataEndOfLastYear = tabsData[Number(key)];
          modifiedDataEndOfLastYear.table.rows.map((el: any, i: number) => {
            return (el.value1 = data.table.rows[i].value4);
          });
          tabsData[Number(key)] = modifiedDataEndOfLastYear;
        }
      }

      setTabsData(fullCopyObject(tabsData));
      setLoaderTable(false);
    }
  };

  const calcRowBind = (bindings: any[], rowArr: any[], rowColIndex: number) => {
    bindings?.forEach((bind) => {
      const operations = bind.operations.split(",");
      const valuesFrom = bind.from.split(",");
      if (bind.type === "row") {
        const targetIndex = rowArr.findIndex(
          (item: any) => bind.target === item.codeItem,
        );
        const targetRow = rowArr[targetIndex];
        const keys = Object.keys(targetRow).filter((key) =>
          key.includes("value"),
        );
        const rowsInCalc = rowArr.filter((item: any) =>
          valuesFrom.includes(item.codeItem),
        );
        keys.forEach((key) => {
          if (
            rowsInCalc.filter((item: any) => item[key] === null).length ===
            rowsInCalc.length
          ) {
            targetRow[key] = null;
          } else {
            targetRow[key] = rowsInCalc.reduce(
              (acc: number, cur: any, index: number) => {
                if (index !== 0) {
                  switch (operations[index - 1]) {
                    case "+":
                      return acc + cur[key];
                    case "-":
                      return acc - cur[key];
                  }
                }
                return acc;
              },
              Number(rowsInCalc[0][key]),
            );
          }
        });
        rowArr.splice(targetIndex, 1, targetRow);
      } else {
        const rowCol = rowArr[rowColIndex];

        if (bind.ignore?.includes(rowCol.codeItem)) return rowArr;

        const calcProps = Object.entries(rowCol).filter(
          (prop) => prop[0].includes("value") && prop[0] !== bind.target,
        );
        if (calcProps.filter((item) => item[1] !== null).length) {
          rowCol[bind.target] = calcProps.reduce(
            (acc: number, cur: any, index: number) => {
              if (index !== 0) {
                switch (operations[index - 1]) {
                  case "+":
                    return acc + cur[1];
                  case "-":
                    return acc - cur[1];
                }
              }
              return acc;
            },
            Number(calcProps[0][1]),
          );
        } else {
          rowCol[bind.target] = null;
        }
        rowArr.splice(rowColIndex, 1, rowCol);
      }
    });
    return rowArr;
  };

  const setData = (tab: number, row: any) => {
    const { bindings } = tabsData[tab];
    const newData = [...tabsData[tab].table.rows];
    const index = newData.findIndex((item) => row.codeItem === item.codeItem);
    const item = newData[index];
    newData.splice(index, 1, {
      ...item,
      ...row,
    });
    tabsData[tab].table.rows = calcRowBind(bindings, newData, index);
    setTabsData(fullCopyObject(tabsData));
  };

  const setAdditionalData = (tab: number, dataObj: any, key: string) => {
    tabsData[tab][key] = dataObj;
    setTabsData(fullCopyObject(tabsData));
  };

  const saveHeader = async (targetForm: any, key: string) => {
    try {
      const value = await targetForm.validateFields();
      setAdditionalData(1, { ...tabsData[1][key], ...value }, key);
    } catch (errInfo) {
      const type = "error";
      const title = "Ошибка сохранения значения";
      const msg =
        "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
  };

  const setNotValid = (tab: number, validObj: any) => {
    const newValidations = tabsData[tab].validations ?? [];
    const validIndex = newValidations.findIndex(
      (item: any) =>
        item.codeItem === validObj.codeItem && item.cell === validObj.cell,
    );
    if (validIndex >= 0) {
      newValidations.splice(validIndex, 1);
    } else {
      newValidations.push(validObj);
    }
    tabsData[tab].validations = newValidations;
    setFlagNotValid(tabsData[tab].validations.length);
    setTabsData(fullCopyObject(tabsData));
  };

  const datePickerRestrict = (date: Moment) => {
    return date.year() > moment(new Date()).year();
  };

  const loadExistReport = async () => {
    if (currPeriod) {
      const saveReportRequest = await getReportByInterval(currPeriod);
      const resultTabs = tabImportConverter(saveReportRequest.tabModels, {
        ...initTabsData,
      });
      if (saveReportRequest.report.attachmentFile?.id) {
        const fileObj = {
          uid: saveReportRequest.report.attachmentFile.id,
          name:
            saveReportRequest.report.attachmentFile.name ||
            "report-document.docx",
          status: "done",
          url: saveReportRequest.report.id,
        };
        setCurrFile(fileObj);
      } else {
        setCurrFile(undefined);
      }
      setExistReport(saveReportRequest);
      setTabsData(resultTabs);
    }
  };

  const postReport = async (isSend: boolean, isCorrect?: boolean) => {
    let reportObj: any;
    let fileInBase64: any;
    let attachmentFile: any;
    if (currFile && !currFile.status) {
      fileInBase64 = await toBase64(currFile);
      attachmentFile = {
        type: currFile.type,
        value: fileInBase64.split("base64,")[1],
        name: currFile.name,
      };
    } else {
      attachmentFile = currFile;
    }
    if (existReport.isSuccess === false) {
      reportObj = {
        report: {
          attachmentFile,
          reportTypeId: initRepType.id,
          userCheckinIntervalId: currPeriod,
        },
        tabModels: Object.values(tabsData).filter((tab: any) => tab !== null),
      };
    } else {
      reportObj = {
        report: {
          ...existReport.report,
          attachmentFile: attachmentFile
            ? { ...attachmentFile, id: attachmentFile.uid }
            : null,
        },
        tabModels: Object.values(tabsData).filter((tab: any) => tab !== null),
      };
    }
    const res = isSend
      ? await sendReport(reportObj)
      : await saveReport(reportObj);
    if (res.isSuccess) {
      setBlock(false);
      if (isSend) {
        await tryExportReport();
        if (isCorrect) {
          const count = await getCountCorrectReports();
          changeReportsRevision(count);
        }
        history.push(`/history-send-reports`);
      } else {
        await loadExistReport();
        const type = "success";
        const title = "Сохранение отчёта выполнено успешно";
        const msg = "";
        openNotification(title, msg, type);
      }
    } else {
      if (res.message === "1488") {
        if (isSend) {
          const type = "warning";
          const title = "Данный отчет уже был отправлен";
          const msg =
            "Будет выполнено автоматическое перенаправление на страницу истории подачи отчетности через 5 секунд.";
          openNotification(title, msg, type);
          setTimeout(() => history.push(`/history-send-reports`), 5000);
        } else {
          setLoaderPage(true);
          await loadExistReport();
          setLoaderPage(false);
          const type = "warning";
          const title = "Данный отчет уже был создан";
          const msg =
            "Была выполнена автоматическая загрузка последнего сохранения данного отчета.";
          openNotification(title, msg, type);
        }
      } else if (res.message === "linkage") {
        setValidError(res.value);
        const type = "error";
        const title = "Присутствуют неувязки между формами";
        const msg =
          "Пожалуйста, откройте меню проверки увязок для ознакомления с ошибками.";
        openNotification(title, msg, type);
      } else {
        const type = "error";
        const title = "Ошибка создания отчета";
        const msg =
          "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
        openNotification(title, msg, type);
      }
    }
  };

  const checkReport = async (isSend: boolean, isCorrect?: boolean) => {
    let reportObj: any;
    let fileInBase64: any;
    let attachmentFile: any;
    if (currFile && !currFile.status) {
      fileInBase64 = await toBase64(currFile);
      attachmentFile = {
        type: currFile.type,
        value: fileInBase64.split("base64,")[1],
        name: currFile.name,
      };
    } else {
      attachmentFile = currFile;
    }
    if (existReport.isSuccess === false) {
      reportObj = {
        report: {
          attachmentFile,
          reportTypeId: initRepType.id,
          userCheckinIntervalId: currPeriod,
        },
        tabModels: Object.values(tabsData).filter((tab: any) => tab !== null),
      };
    } else {
      reportObj = {
        report: {
          ...existReport.report,
          attachmentFile: attachmentFile
            ? { ...attachmentFile, id: attachmentFile.uid }
            : null,
        },
        tabModels: Object.values(tabsData).filter((tab: any) => tab !== null),
      };
    }
    const res = isSend
      ? await validateReport(reportObj)
      : await validateReport(reportObj);
    if (res.isSuccess) {
      setBlock(false);
      setValidError([]);
      const type = "success";
      const title =
        "Увязки между формами осуществлены успешно. Неувязки отсутствуют.";
      const msg = "";
      openNotification(title, msg, type);
    } else {
      if (res.message === "linkage") {
        setValidError(res.value);
        const type = "error";
        const title = "Присутствуют неувязки между формами";
        const msg =
          "Пожалуйста, откройте меню проверки увязок для ознакомления с ошибками.";
        openNotification(title, msg, type);
      } else {
        setValidError([]);
        const type = "error";
        const title = "Ошибка проверки отчета";
        const msg =
          "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
        openNotification(title, msg, type);
      }
    }
  };

  const tryCheckReport = async () => {
    setLoaderPage(true);
    await checkReport(false);
    setLoaderPage(false);
  };

  const trySaveReport = async () => {
    setLoaderPage(true);
    await postReport(false);
    setLoaderPage(false);
  };

  const trySendReport = async () => {
    if (idOrganization) {
      const generalInfo = await getGeneralInfo(idOrganization);

      if (currFile && generalInfo.value.unp) {
        setLoaderPage(true);
        await postReport(true);
        setLoaderPage(false);
      } else if (!generalInfo.value.unp) {
        const type = "error";
        const title = "Ошибка отправки отчёта";
        const msg =
          "Пожалуйста, добавьте данные об организации во вкладке 'общие сведения'";
        openNotification(title, msg, type);
      } else {
        const type = "error";
        const title = "Ошибка отправки отчёта";
        const msg = "Пожалуйста, добавьте файл приложения";
        openNotification(title, msg, type);
      }
    }
  };

  const trySendCorrectReport = async () => {
    if (currFile) {
      setLoaderPage(true);
      await postReport(true, true);
      setLoaderPage(false);
    } else {
      const type = "error";
      const title = "Ошибка отправки отчёта";
      const msg = "Пожалуйста, добавьте файл приложения";
      openNotification(title, msg, type);
    }
  };

  const tryAcceptReport = async (note: string) => {
    setLoaderPage(true);
    const acceptObj = {
      report: {
        ...existReport.report,
        note,
        statusReport: 2,
        adminStatusReport: 3,
      },
      tabModels: existReport.tabModels,
    };
    const res = await acceptReport(acceptObj);
    if (res.isSuccess) {
      history.push("/accepted-reports");
    } else {
      const type = "error";
      const title = "Ошибка принятия отчета";
      const msg =
        "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    setLoaderPage(false);
  };

  const trySendToCorrectReport = async (note: string) => {
    setLoaderPage(true);
    const correctReportObj = {
      report: {
        ...existReport.report,
        note,
        statusReport: 3,
        adminStatusReport: 2,
      },
      tabModels: Object.values(tabsData).filter((tab: any) => tab !== null),
    };
    const res = await sendToCorrectReport(correctReportObj);
    if (res.isSuccess) {
      history.push("/");
    } else {
      const type = "error";
      const title = "Ошибка отправления отчета на корректировку";
      const msg =
        "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    setLoaderPage(false);
  };

  const tabImportConverter = (tabsArr: any[] = [], tabObj: any) => {
    tabsArr.forEach((item) => {
      tabObj[item.tabId] = item;
    });
    return tabObj;
  };

  const tryImportReport = async (file: any) => {
    setLoaderPage(true);
    const fileInBase64: any = await toBase64(file);
    const strFileInBase64 = fileInBase64.split("base64,")[1];
    const res = await importReportByQuarter(strFileInBase64, currPeriod ?? "");
    if (res.isSuccess === false) {
      const type = "error";
      const title = "Ошибка импорта отчета";
      const msg =
        "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    } else {
      const processedTabs = res.map((tab: any) => {
        const {
          bindings,
          table: { rows: newData },
        } = tab;
        let resultArr: any[] = newData;
        if (bindings) {
          newData.forEach((item: any, index: number) => {
            if (newData[index].codeItem) {
              resultArr = calcRowBind(bindings, resultArr, index);
            }
          });
        }
        tab.table.rows = resultArr;
        return tab;
      });
      const resultTabs = tabImportConverter(processedTabs, { ...initTabsData });

      const dataPage1 = endOfLastYear?.filter((el: any) => el.tabId === 1)[0];
      if (dataPage1) {
        resultTabs[1].table.rows.map(
          (el: any, i: number) => (el.value2 = dataPage1.table.rows[i].value2),
        );
      }

      const dataPage2 = samePeriodLastYear?.filter(
        (el: any) => el.tabId === 2,
      )[0];
      if (dataPage2) {
        resultTabs[2].table.rows.map(
          (el: any, i: number) => (el.value2 = dataPage2.table.rows[i].value2),
        );
      }

      const dataPage4 = samePeriodLastYear?.filter(
        (el: any) => el.tabId === 4,
      )[0];
      if (dataPage4) {
        resultTabs[4].table.rows.map(
          (el: any, i: number) => (el.value2 = dataPage4.table.rows[i].value2),
        );
      }

      const dataPage6 = endOfLastYear?.filter((el: any) => el.tabId === 6)[0];
      if (dataPage6) {
        resultTabs[6].table.rows.map(
          (el: any, i: number) => (el.value1 = dataPage6.table.rows[i].value1),
        );
      }

      setTabsData(resultTabs);
      const type = "success";
      const title = "Импорт документа выполнен успешно";
      const msg = "";
      openNotification(title, msg, type);
    }
    setValidError([]);
    setLoaderPage(false);
  };

  const tryExportReport = async (full?: boolean) => {
    setLoaderPage(true);
    let res;
    if (existReport?.report?.id && !full) {
      res = await exportById(existReport.report.id);
    } else {
      const reportObj: any = {
        report: {
          reportTypeId: initRepType.id,
          userCheckinIntervalId: currPeriod,
        },
        tabModels: Object.values(tabsData).filter((tab: any) => tab !== null),
        validOrganization: existReport?.validOrganization
          ? existReport?.validOrganization
          : null,
        invalidOrganization: existReport?.invalidOrganization
          ? existReport?.invalidOrganization
          : null,
      };
      res = await exportReport(reportObj);
    }
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
    setLoaderPage(false);
  };

  const headerReportTable = (
    <div
      className={
        check ? "check-form header-report-table" : "header-report-table"
      }
    >
      {reportId && user?.role === "Admin" && (
        <Form form={headerForm}>
          <div className={"table-header-row"}>
            <span>Организация</span>
            <Form.Item name={"organization"}>
              <Input
                readOnly={!vault}
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
              />
            </Form.Item>
          </div>
          <div className={"table-header-row"}>
            <span>Учетный номер плательщика</span>
            <Form.Item name={"number"}>
              <Input
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
                readOnly={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-row"}>
            <span>Вид экономической деятельности</span>
            <Form.Item name={"typeEconomicActivity"}>
              <Input
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
                readOnly={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-row"}>
            <span>Организационно-правовая форма</span>
            <Form.Item name={"organizationalLegalForm"}>
              <Input
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
                readOnly={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-row"}>
            <span>Орган управления</span>
            <Form.Item name={"government"}>
              <Input
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
                readOnly={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-row"}>
            <span>Единица измерения</span>
            <Form.Item name={"unit"}>
              <Input
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
                readOnly={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-row"}>
            <span>Адрес</span>
            <Form.Item name={"address"}>
              <Input
                onPressEnter={() => saveHeader(headerForm, "header")}
                onBlur={() => saveHeader(headerForm, "header")}
                readOnly={check}
              />
            </Form.Item>
          </div>
        </Form>
      )}
      {Number(activeTab) === 1 ? (
        <Form form={additionalForm} className={"table-header-date"}>
          <div className={"table-header-date-row"}>
            <span>Дата утверждения</span>
            <Form.Item name={"approvedDate"}>
              <DatePicker
                format={"DD.MM.YYYY"}
                onBlur={() => saveHeader(additionalForm, "additionTable")}
                disabled={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-date-row"}>
            <span>Дата отправки</span>
            <Form.Item name={"sendDate"}>
              <DatePicker
                format={"DD.MM.YYYY"}
                onBlur={() => saveHeader(additionalForm, "additionTable")}
                disabled={check}
              />
            </Form.Item>
          </div>
          <div className={"table-header-date-row"}>
            <span>Дата принятия</span>
            <Form.Item name={"acceptedDate"}>
              <DatePicker
                format={"DD.MM.YYYY"}
                onBlur={() => saveHeader(additionalForm, "additionTable")}
                disabled={check}
              />
            </Form.Item>
          </div>
        </Form>
      ) : (
        ""
      )}
    </div>
  );

  return vault ? (
    <div className={"content-wrap report-view"}>
      {loaderPage ? (
        <div
          style={{ opacity: 0.5 }}
          className={"loading-page flex-row --row-center"}
        >
          <Spin size="large" />
        </div>
      ) : existReport ? (
        <Layout.Content style={{ paddingTop: "1px" }}>
          {headerReportTable}
          <Tabs type={"card"} onTabClick={loadTabData}>
            {tabs.map((item: any, index: number) => (
              <Tabs.TabPane tab={item} key={index + 1}>
                <div
                  style={{
                    height: "100%",
                    minHeight: "720px",
                    background: "rgba(184, 178, 173, 0.8)",
                    padding: "20px",
                  }}
                >
                  {loaderTable ? (
                    <div
                      className={"loading-page flex-row --row-center"}
                      style={{ zIndex: 10 }}
                    >
                      <Spin size="large" />
                    </div>
                  ) : (
                    <ReportTable
                      tabIndex={index + 1}
                      tabData={tabsData[index + 1]}
                      setData={setData}
                      setAdditionalData={setAdditionalData}
                      setNotValid={setNotValid}
                      isCheck={check}
                      isVault={vault}
                      isCorrect={correct}
                    />
                  )}
                </div>
              </Tabs.TabPane>
            ))}
          </Tabs>
        </Layout.Content>
      ) : (
        <Empty
          style={{ minHeight: "calc(100vh - 150px)" }}
          className={"flex-row --row-center --flex-col "}
        />
      )}
      <span className={"fixed-drawer-button --export"}>
        <Button
          size={"large"}
          type="primary"
          title={"Экспорт свода"}
          icon={<ExportOutlined style={{ fontSize: "24px" }} />}
          disabled={
            !(
              existReport?.invalidOrganization && existReport?.validOrganization
            )
          }
          onClick={() => tryExportReport(true)}
        />
      </span>
      <span className={"fixed-drawer-button"}>
        <Button
          size={"large"}
          type="primary"
          title={"Организации в своде"}
          icon={<FileDoneOutlined style={{ fontSize: "24px" }} />}
          onClick={showDrawer}
        />
      </span>
      <Drawer
        style={{ zIndex: 1000000 }}
        width={550}
        title="Организаций в своде"
        placement="right"
        closable={true}
        onClose={onClose}
        visible={visible}
      >
        {existReport?.invalidOrganization && existReport?.validOrganization ? (
          <>
            <h3>Включены:</h3>
            <ol>
              {existReport.validOrganization.map((item: any) => (
                <li style={{ paddingBottom: "15px" }}>{item.name}</li>
              ))}
            </ol>
            <h3>Не включены:</h3>
            <ol>
              {existReport.invalidOrganization.map((item: any) => (
                <li style={{ paddingBottom: "15px" }}>{item.name}</li>
              ))}
            </ol>
          </>
        ) : (
          <p style={{ textAlign: "center" }}>
            За данный квартал отсутствуют сданные отчеты от организаций.
          </p>
        )}
      </Drawer>
    </div>
  ) : (
    <div className={"content-wrap report-view"}>
      <Prompt
        when={block}
        message={() =>
          `Не сохраненные данные будут потеряны. Вы уверены, что хотите покинуть страницу?`
        }
      />
      {loaderPage ? (
        <div
          style={{ opacity: 0.5 }}
          className={"loading-page flex-row --row-center"}
        >
          <Spin size="large" />
        </div>
      ) : periods.filter((item: Period) => item.isFree).length ||
        check ||
        correct ? (
        <Layout>
          <Layout style={{ background: "white" }}>
            <Layout.Header
              style={{
                height: "auto",
                minHeight: "85px",
                background: "inherit",
                padding: "20px 20px 0",
              }}
            >
              <TopControlPanel
                periods={periods}
                onYearStateChange={onYearChange}
                onPeriodStateChange={onPeriodChange}
                onNoteStateChange={onNoteChange}
                onUploadFileChange={onUploadFileChange}
                currPeriod={currPeriod}
                currYear={currYear}
                currFile={currFile}
                loaderPage={loaderPage}
                flagNotValid={flagNotValid}
                isCheck={check}
                isCorrect={correct}
                initNote={note ?? existReport?.report?.note ?? ""}
                trySaveReport={trySaveReport}
                trySendReport={trySendReport}
                tryExportReport={tryExportReport}
                tryImportReport={tryImportReport}
                trySendToCorrectReport={trySendToCorrectReport}
                tryAcceptReport={tryAcceptReport}
                trySendCorrectReport={trySendCorrectReport}
                tryCheckReport={tryCheckReport}
              />
            </Layout.Header>
            <Layout.Content style={{ padding: "0 20px 20px" }}>
              {headerReportTable}
              <Tabs
                type={"card"}
                activeKey={activeTab}
                onTabClick={loadTabData}
              >
                {tabs.map((item: any, index: number) => (
                  <Tabs.TabPane tab={item} key={index + 1}>
                    <div
                      style={{
                        height: "100%",
                        minHeight: "720px",
                        background: "rgba(184, 178, 173, 0.8)",
                        padding: "20px",
                      }}
                    >
                      {loaderTable ? (
                        <div
                          className={"loading-page flex-row --row-center"}
                          style={{ zIndex: 10 }}
                        >
                          <Spin size="large" />
                        </div>
                      ) : (
                        <ReportTable
                          tabIndex={index + 1}
                          tabData={tabsData[index + 1]}
                          setData={setData}
                          setAdditionalData={setAdditionalData}
                          setNotValid={setNotValid}
                          isCheck={check}
                          isVault={false}
                          isCorrect={correct}
                        />
                      )}
                    </div>
                  </Tabs.TabPane>
                ))}
              </Tabs>
              {check ? null : (
                <>
                  <span className={"fixed-drawer-button"}>
                    <Badge count={validError.length}>
                      <Button
                        size={"large"}
                        type="primary"
                        title={"Проверка увязок между формами"}
                        icon={<WarningOutlined style={{ fontSize: "24px" }} />}
                        onClick={showDrawer}
                      />
                    </Badge>
                  </span>
                  <Drawer
                    style={{ zIndex: 1000000 }}
                    width={500}
                    title="Проверка увязок между формами"
                    placement="right"
                    closable={true}
                    onClose={onClose}
                    visible={visible}
                  >
                    {validError.length ? (
                      <ol>
                        {validError.map((err) => (
                          <li style={{ paddingBottom: "15px" }}>{err}</li>
                        ))}
                      </ol>
                    ) : (
                      <p style={{ textAlign: "center" }}>
                        Ошибки валидации увязок отсутствуют. <br /> Отправьте
                        отчет для прохождения валидации.
                      </p>
                    )}
                  </Drawer>
                </>
              )}
            </Layout.Content>
          </Layout>
        </Layout>
      ) : (
        <div
          key={"result-container"}
          style={{ zIndex: 10 }}
          className={"loading-page flex-row --row-center"}
        >
          <Result
            status="success"
            title="Отсутствуют периоды для сдачи. "
            subTitle="Попробуйте выбрать другой год."
            extra={[
              <Button
                key={"redirect"}
                type="primary"
                onClick={() => {
                  setBlock(true);
                  history.push(`/history-send-reports`);
                }}
              >
                Перейти к истории подачи отчетности
              </Button>,
              <DatePicker
                key={"datepicker"}
                allowClear={false}
                className={"deadlines__calendar"}
                value={currYear}
                picker="year"
                disabledDate={datePickerRestrict}
                onChange={(value) => setCurrYear(value)}
                placeholder={"Выберите год..."}
                style={{ maxWidth: "200px" }}
              />,
            ]}
          />
        </div>
      )}
    </div>
  );
};

export default ReportView;
