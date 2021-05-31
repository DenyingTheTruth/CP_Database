import { url } from "../../constants/constants";
import {
  token,
  fnGetFileNameFromContentDispositionHeader,
} from "../../helpers/helpers";

const getBalanceAsset = async (data: any, mode: string) => {
  return await fetch(`${url}/analytics/${mode}-balance-asset`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())

    .catch((err) => {
      console.log(err);
    });
};

const getBalanceLiabilities = async (data: any, mode: string) => {
  return await fetch(`${url}/analytics/${mode}-balance-liabilities`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())

    .catch((err) => {
      console.log(err);
    });
};

const getFinancialIndicators = async (data: any, mode: string) => {
  return await fetch(`${url}/analytics/${mode}-financial-indicators`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())
    .catch((err) => {
      console.log(err);
    });
};

const getStructureLiabilities = async (data: any, mode: string) => {
  return await fetch(`${url}/analytics/${mode}-structure-obligations`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())
    .catch((err) => {
      console.log(err);
    });
};

const getSolvencyRatios = async (data: any, mode: string) => {
  return await fetch(`${url}/analytics/${mode}-solvency-ratios`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())
    .catch((err) => {
      console.log(err);
    });
};

const getWorkingCapital = async (data: any, mode: string) => {
  return await fetch(`${url}/analytics/${mode}-working-capital`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((res) => res.json())
    .catch((err) => {
      console.log(err);
    });
};

const exportAnalytics = async (currReport: number, data: any, mode: string) => {
  return await fetch(
    `${url}/analytics/${mode}-export?analyticsTypeEnum=${currReport}`,
    {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token()}`,
      },
      body: JSON.stringify(data),
    },
  )
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
    .catch((err) => {
      console.log(err);
    });
};

export {
  getBalanceAsset,
  getBalanceLiabilities,
  getFinancialIndicators,
  getStructureLiabilities,
  getSolvencyRatios,
  getWorkingCapital,
  exportAnalytics,
};
