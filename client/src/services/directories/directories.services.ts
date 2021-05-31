import { url } from "../../constants/constants";
import { token } from "../../helpers/helpers";

const getReportTypes = async (id?: string) => {
  return await fetch(`${url}/report-types/${id || ""}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => {
      if (!res.errorText) {
        return res;
      }
    })
    .catch((err) => err);
};

const getReportTypeByReportName = async (reportName: string) => {
  return await fetch(`${url}/report-type/${reportName}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => res)
    .catch((err) => err);
};

export { getReportTypes, getReportTypeByReportName };
