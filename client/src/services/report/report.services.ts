import { url } from "../../constants/constants";
import {
  fnGetFileNameFromContentDispositionHeader,
  token,
} from "../../helpers/helpers";

async function getUserReports(signal?: AbortSignal): Promise<any> {
  return await fetch(`${url}/reports`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => {
      if (!result.errorText) {
        return result;
      }
    })
    .catch((err) => {
      console.log(err);
    });
}

const getReportById = async (id: string, signal?: AbortSignal) => {
  return await fetch(`${url}/reports/${id}`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => {
      if (!result.errorText) {
        return result;
      }
    })
    .catch((err) => {
      console.log(err);
    });
};

const getReportByInterval = async (id: string, signal?: AbortSignal) => {
  return await fetch(`${url}/report-by-interval/${id}`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => {
      if (!result.errorText) {
        return result;
      }
    })
    .catch((err) => {
      console.log(err);
    });
};

const getUserReportsRevision = async (signal?: AbortSignal): Promise<any> => {
  return await fetch(`${url}/reports-revision`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => {
      if (!result.errorText) {
        return result;
      }
    })
    .catch((err) => {
      console.log(err);
    });
};

async function getReportTabs(
  reportTypeName: string,
  index?: number,
  signal?: AbortSignal,
) {
  return await fetch(
    `${url}/reports/tab/${reportTypeName}${
      typeof index !== "undefined" ? `?number=${index}` : ""
    }`,
    {
      method: "GET",
      signal: signal,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token()}`,
      },
    },
  )
    .then((response) => response.json())
    .then((result) => result)
    .catch((err) => err);
}

async function getFreeReport(
  year: number,
  reportTypeId: string,
  signal?: AbortSignal,
) {
  return await fetch(`${url}/reports/free/${year},${reportTypeId}`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => result)
    .catch((err) => err);
}

async function getReportTab(
  reportTypeName: string,
  index: number,
  userCheckinIntervalId: string | undefined,
  signal?: AbortSignal,
) {
  return await fetch(
    `${url}/reports/tab/${reportTypeName}?number=${index}&intervalId=${userCheckinIntervalId}`,
    {
      method: "GET",
      signal: signal,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token()}`,
      },
    },
  )
    .then((response) => response.json())
    .then((result) => result)
    .catch((err) => err);
}

async function getSentReports(signal?: AbortSignal) {
  return await fetch(`${url}/reports/sent`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => result)
    .catch((err) => err);
}

async function saveReport(report: any, signal?: AbortSignal) {
  return await fetch(`${url}/reports/save`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(report),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function acceptReport(report: any, signal?: AbortSignal) {
  return await fetch(`${url}/reports/accept`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(report),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function sendToCorrectReport(report: any, signal?: AbortSignal) {
  return await fetch(`${url}/reports/send-to-correction`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(report),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function exportReport(report: any, signal?: AbortSignal) {
  return await fetch(`${url}/reports/export`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(report),
  })
    .then(async (res) => ({
      filename: fnGetFileNameFromContentDispositionHeader(
        res?.headers.get("content-disposition"),
      ),
      blob: await res.blob(),
    }))
    .then((fileObj) => {
      try {
        const url = URL.createObjectURL(fileObj.blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = fileObj.filename;
        link.click();
        link.remove();
        return { isSuccess: true };
      } catch (e) {
        return { isSuccess: false, msg: e };
      }
    })
    .catch((err) => err);
}

async function downloadFile(
  reportId: string,
  name: string,
  signal?: AbortSignal,
) {
  return await fetch(`${url}/reports/file/${reportId}`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then(async (res) => ({
      filename: fnGetFileNameFromContentDispositionHeader(
        res?.headers.get("content-disposition"),
      ),
      blob: await res.blob(),
    }))
    .then((fileObj) => {
      try {
        const url = URL.createObjectURL(fileObj.blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = name;
        link.click();
        link.remove();
        return { isSuccess: true };
      } catch (e) {
        return { isSuccess: false, msg: e };
      }
    })
    .catch((err) => err);
}

async function importReport(fileInBase64: string, signal?: AbortSignal) {
  return await fetch(`${url}/reports/import`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(fileInBase64),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function importReportByQuarter(
  fileInBase64: string,
  id: string,
  signal?: AbortSignal,
) {
  return await fetch(`${url}/reports/import-by-quarter`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify({
      file: fileInBase64,
      id: id,
    }),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function sendReport(report: any, signal?: AbortSignal) {
  return await fetch(`${url}/reports/send`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(report),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function validateReport(report: any, signal?: AbortSignal) {
  return await fetch(`${url}/reports/validate`, {
    method: "POST",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(report),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

const getUserReportLogs = async (signal?: AbortSignal) => {
  return await fetch(`${url}/reports/logs`, {
    method: "GET",
    signal: signal,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => {
      if (!result.errorText) {
        return result;
      }
    })
    .catch((err) => console.log(err));
};

const getAcceptedReports = async (year: number, reportTypeId: string) => {
  return await fetch(`${url}/reports/accepted/${year},${reportTypeId}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((response) => response.json())
    .then((result) => {
      if (!result.errorText) {
        return result;
      }
    })
    .catch((err) => {
      console.log(err);
    });
};

const sendReportsToCorrection = async (reports: Array<string>) => {
  return await fetch(`${url}/reports/return-reports-to-correction`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(reports),
  });
};

const exportById = async (reportId: string) => {
  return await fetch(`${url}/reports/export-by-id/${reportId}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then(async (res) => ({
      filename: fnGetFileNameFromContentDispositionHeader(
        res?.headers.get("content-disposition"),
      ),
      blob: await res.blob(),
    }))
    .then((fileObj) => {
      try {
        const url = URL.createObjectURL(fileObj.blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = fileObj.filename;
        link.click();
        link.remove();
        return { isSuccess: true };
      } catch (e) {
        return { isSuccess: false, msg: e };
      }
    })
    .catch((err) => err);
};

async function getCountCorrectReports() {
  return await fetch(`${url}/reports/count-corrections`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function getCountUnreadReports() {
  return await fetch(`${url}/reports/count-unread-reports`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

const getConcernVault = async (data: any) => {
  return await fetch(`${url}/reports/statistics`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
}

async function readReport(reportId: string ) {
  return await fetch(`${url}/reports/read/${reportId}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
  .catch((err) => {
    console.log(err);
  });
}

const getDataByYearPeriodReportId = async (
  year: number,
  periodId: string,
  reportTypeId: string,
  organization: string,
) => {
  return await fetch(
    `${url}/reports/${year},${periodId},${reportTypeId},${organization}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token()}`,
      },
    },
  )
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
};

export {
  getUserReports,
  getReportById,
  getReportByInterval,
  getUserReportsRevision,
  getReportTabs,
  getFreeReport,
  getReportTab,
  getSentReports,
  saveReport,
  sendReport,
  exportReport,
  importReport,
  importReportByQuarter,
  getUserReportLogs,
  downloadFile,
  acceptReport,
  sendToCorrectReport,
  getAcceptedReports,
  sendReportsToCorrection,
  getCountCorrectReports,
  getCountUnreadReports,
  exportById,
  getConcernVault,
  validateReport,
  getDataByYearPeriodReportId,
  readReport,
};
