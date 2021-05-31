import { url } from "../../constants/constants";
import { token } from "../../helpers/helpers";

const getIntervals = async (year: number, reportTypeId: string) => {
  return await fetch(`${url}/intervals/${year},${reportTypeId}`, {
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
    .catch((err) => console.log(err));
};

const changeInterval = async (data: any) => {
  return await fetch(`${url}/intervals`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((response) => response.json())
    .then((result) => result)
    .catch((err) => console.log(err));
};

const copyInterval = async (data: any) => {
  return await fetch(`${url}/intervals/copy`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(data),
  })
    .then((response) => response.json())
    .then((result) => result)
    .catch((err) => console.log(err));
};

export { getIntervals, changeInterval, copyInterval };
