import React, { useState } from "react";
import { Button, DatePicker, Select, Upload, Input } from "antd";
import moment, { Moment } from "moment";
import { Period } from "../../../../../models/common-types";
import {
  ExportOutlined,
  ImportOutlined,
  PaperClipOutlined,
  SaveOutlined,
  SendOutlined,
  CheckOutlined,
  RollbackOutlined,
} from "@ant-design/icons";
import { acceptedFiles, importFiles } from "../../../../../constants/constants";
import { openNotification } from "../../../../../helpers/helpers";
import { downloadFile } from "../../../../../services/report/report.services";

const { Option } = Select;

const TopControlPanel = ({
  currYear,
  onYearStateChange,
  periods,
  onPeriodStateChange,
  onNoteStateChange,
  currPeriod,
  trySaveReport,
  currFile,
  onUploadFileChange,
  tryExportReport,
  trySendReport,
  tryImportReport,
  loaderPage,
  isCheck,
  isCorrect,
  trySendToCorrectReport,
  flagNotValid,
  initNote,
  tryAcceptReport,
  trySendCorrectReport,
  tryCheckReport,
}: {
  currPeriod: string | undefined;
  isCheck: boolean;
  isCorrect: boolean;
  flagNotValid: number;
  currYear: Moment | null;
  onYearStateChange: (value: Moment | null) => void;
  periods: Array<Period>;
  onPeriodStateChange: (value: string) => void;
  onNoteStateChange: (value: string) => void;
  trySaveReport: () => void;
  tryCheckReport: () => void;
  currFile: any;
  initNote: string;
  tryExportReport: (flag: boolean) => void;
  trySendReport: () => void;
  trySendToCorrectReport: (note: string) => void;
  tryAcceptReport: (note: string) => void;
  tryImportReport: (file: any) => void;
  trySendCorrectReport: () => void;
  onUploadFileChange: (file: any) => void;
  loaderPage: boolean;
}) => {
  const [note, setNote] = useState<string>(initNote);

  const onYearChange = (value: Moment | null): void => {
    onYearStateChange(value);
  };

  const onPeriodChange = (value: string) => {
    onPeriodStateChange(value);
  };

  const exportReport = () => {
    tryExportReport(true);
  };

  const exportReportFull = () => {
    tryExportReport(false);
  };

  const saveReport = () => {
    trySaveReport();
  };

  const checkReport = () => {
    tryCheckReport();
  };

  const sendReport = () => {
    trySendReport();
  };

  const sendCorrectReport = () => {
    trySendCorrectReport();
  };

  const datePickerRestrict = (date: Moment) => {
    return date.year() > moment(new Date()).year();
  };

  const acceptReport = () => {
    tryAcceptReport(note);
  };

  const sendToCorrectReport = () => {
    trySendToCorrectReport(note);
  };

  const onNoteChange = (e: any) => {
    setNote(e.target.value);
    onNoteStateChange(e.target.value);
  };

  const importProps: any = {
    fileList: [],
    disabled: loaderPage,
    accept: importFiles,
    beforeUpload: async (file: any) => {
      if (importFiles.includes(file.type)) {
        tryImportReport(file);
      } else {
        const type = "error";
        const title = "Ошибка импорта файла";
        const msg =
          "Неверный тип файла. Пожалуйста, выберете только файл формат .xls или .xlsx.";
        openNotification(title, msg, type);
      }
      return false;
    },
    onChange(info: any) {
      if (info.file.status !== "uploading") {
        console.log(info.file, info.fileList);
      }
      if (info.file.status === "done") {
        console.log(`${info.file.name} file uploaded successfully`);
      } else if (info.file.status === "error") {
        console.log(`${info.file.name} file upload failed.`);
      }
    },
  };

  const attachFileProps: any = {
    className: "file-wrapper",
    fileList: currFile ? [currFile] : [],
    disabled: loaderPage,
    accept: acceptedFiles,
    onRemove: () => {
      onUploadFileChange(undefined);
    },
    onPreview: async (file: any) => {
      if (file.status) {
        const res = await downloadFile(file.url, file.name);
        if (res.isSuccess) {
          const type = "success";
          const title = "Скачивание файла выполнено успешно";
          const msg = "";
          openNotification(title, msg, type);
        } else {
          const type = "error";
          const title = "Ошибка скачивания файла";
          const msg =
            "Возможна проблема с сервером. Также, пожалуйста, проверьте интернет-подключение.";
          openNotification(title, msg, type);
        }
      } else {
        const blobFile = new Blob([file]);
        const url = URL.createObjectURL(blobFile);
        const link = document.createElement("a");
        link.href = url;
        link.download = file.name;
        link.click();
        link.remove();
      }
    },
    beforeUpload: (file: any) => {
      if (acceptedFiles.includes(file.type)) {
        onUploadFileChange(file);
      } else {
        const type = "error";
        const title = "Ошибка добавление файла";
        const msg =
          "Неверный тип файла. Пожалуйста, выберете только файл формат .doc или .docx.";
        openNotification(title, msg, type);
      }
      if (file.size < 15728640) {
        onUploadFileChange(file);
      } else {
        const type = "error";
        const title = "Ошибка добавление файла";
        const msg = "Размер файла не должен превышать 15MB";
        openNotification(title, msg, type);
        onUploadFileChange(undefined);
      }
      return false;
    },
  };

  return (
    <div className={"flex-row top-control-wrap"}>
      {isCheck ? (
        <div className={"top-control-wrap--check"}>
          <div className={"flex-row"}>
            <Input.TextArea
              value={note}
              onChange={onNoteChange}
              placeholder={"Примечание"}
              rows={5}
            />
          </div>
          <div className={"flex-row"}>
            <Button
              icon={<ExportOutlined style={{ fontSize: "18px" }} />}
              onClick={exportReportFull}
              className={"flex-row"}
              disabled={loaderPage}
            >
              <span>Экспорт</span>
            </Button>
            <Button
              icon={<RollbackOutlined style={{ fontSize: "18px" }} />}
              onClick={sendToCorrectReport}
              className={"flex-row"}
              disabled={(!note && !flagNotValid) || loaderPage}
            >
              <span>Отправить на корректировку</span>
            </Button>
            <Button
              icon={<CheckOutlined style={{ fontSize: "18px" }} />}
              type={"primary"}
              onClick={acceptReport}
              disabled={loaderPage || !!flagNotValid}
              className={"flex-row"}
            >
              <span>Принять</span>
            </Button>
          </div>
        </div>
      ) : isCorrect ? (
        <div className={"top-control-wrap--check"}>
          <div className={"flex-row"}>
            <Input.TextArea
              value={note}
              readOnly={true}
              placeholder={"Примечание"}
              rows={5}
            />
          </div>
          <div className={"flex-row"}>
            <Button
              icon={<ExportOutlined style={{ fontSize: "18px" }} />}
              onClick={exportReport}
              className={"flex-row"}
              disabled={loaderPage}
            >
              <span>Экспорт</span>
            </Button>
            <Upload {...attachFileProps}>
              <Button
                icon={<PaperClipOutlined style={{ fontSize: "18px" }} />}
                className={"flex-row"}
              >
                <span>Добавить приложение</span>
              </Button>
            </Upload>
            <Button
              icon={<SaveOutlined style={{ fontSize: "18px" }} />}
              onClick={saveReport}
              className={"flex-row"}
              disabled={loaderPage}
            >
              <span>Сохранить</span>
            </Button>
            <Button
              icon={<SendOutlined style={{ fontSize: "18px" }} />}
              type={"primary"}
              onClick={sendCorrectReport}
              disabled={loaderPage}
              className={"flex-row"}
            >
              <span>Отправить</span>
            </Button>
          </div>
        </div>
      ) : (
        <>
          <div style={{ width: "360px" }} className={"flex-row"}>
            <DatePicker
              allowClear={false}
              className={"deadlines__calendar"}
              value={currYear}
              picker="year"
              disabled={loaderPage}
              disabledDate={datePickerRestrict}
              onChange={onYearChange}
              placeholder={"Выберите год..."}
              style={{ maxWidth: "200px" }}
            />
            <Select
              value={currPeriod}
              onChange={onPeriodChange}
              className={"width-100"}
              style={{ maxWidth: "200px" }}
              disabled={loaderPage}
              placeholder={"Выберите период..."}
            >
              {periods.map((item) => (
                <Option
                  key={item.userCheckinIntervalId}
                  value={item.userCheckinIntervalId}
                  disabled={!item.isFree}
                >
                  {item.name}
                </Option>
              ))}
            </Select>
          </div>
          <div style={{ width: "900px" }} className={"flex-row"}>
            <Button
              onClick={checkReport}
              className={"flex-row"}
              disabled={loaderPage}
            >
              <span>Проверка увязок</span>
            </Button>
            <Upload {...importProps}>
              <Button
                icon={<ImportOutlined style={{ fontSize: "18px" }} />}
                className={"flex-row"}
              >
                <span>Импорт</span>
              </Button>
            </Upload>
            <Button
              icon={<ExportOutlined style={{ fontSize: "18px" }} />}
              onClick={exportReport}
              className={"flex-row"}
              disabled={loaderPage}
            >
              <span>Экспорт</span>
            </Button>
            <Upload {...attachFileProps}>
              <Button
                icon={<PaperClipOutlined style={{ fontSize: "18px" }} />}
                className={"flex-row"}
              >
                <span>Добавить приложение</span>
              </Button>
            </Upload>
            <Button
              icon={<SaveOutlined style={{ fontSize: "18px" }} />}
              onClick={saveReport}
              className={"flex-row"}
              disabled={loaderPage}
            >
              <span>Сохранить</span>
            </Button>
            <Button
              icon={<SendOutlined style={{ fontSize: "18px" }} />}
              type={"primary"}
              onClick={sendReport}
              disabled={loaderPage}
              className={"flex-row"}
            >
              <span>Отправить</span>
            </Button>
          </div>
        </>
      )}
    </div>
  );
};

export default TopControlPanel;
