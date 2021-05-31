import { tokenName } from "../constants/constants";
import { notification } from "antd";

function token(): string | null {
  const crmCookie = document.cookie
    .split(" ")
    .map((item) => (item.split("=")[0] === tokenName ? item : null))
    .filter((item) => !!item);
  return crmCookie[0] ? crmCookie[0].slice(tokenName.length + 1) : null;
}

function setCookie(name: string, value: string, options: any) {
  options = {
    path: "/",
    samesite: "strict",
    ...options,
  };

  let updatedCookie = `${encodeURIComponent(name)}=${encodeURIComponent(
    value,
  )}`;

  for (let optionKey in options) {
    updatedCookie += "; " + optionKey;
    let optionValue = options[optionKey];
    if (optionValue !== true) {
      updatedCookie += "=" + optionValue;
    }
  }

  document.cookie = updatedCookie;
}

function openNotification(title: string, msg: string, type: string) {
  // @ts-ignore
  notification[type]({
    message: title,
    description: msg,
    placement: "bottomLeft",
  });
}

const toBase64 = (file: any) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });

const fullCopyObject = (obj: any) => JSON.parse(JSON.stringify(obj));

const fnGetFileNameFromContentDispositionHeader = (header: string | null) => {
  const contentDisposition = header?.split(";");
  const fileNameToken = `filename*=UTF-8''`;

  let fileName = "export-report.xlsx";
  if (contentDisposition) {
    for (let thisValue of contentDisposition) {
      if (thisValue.trim().indexOf(fileNameToken) === 0) {
        fileName = decodeURIComponent(
          thisValue.trim().replace(fileNameToken, ""),
        );
        break;
      }
    }
  }

  return fileName;
};

export {
  setCookie,
  token,
  fullCopyObject,
  openNotification,
  toBase64,
  fnGetFileNameFromContentDispositionHeader,
};
