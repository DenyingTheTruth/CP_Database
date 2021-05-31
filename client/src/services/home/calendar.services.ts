import { url } from "../../constants/constants";
import { token } from "../../helpers/helpers";
import { ICalendarObject } from "../../models/common-interfaces";

const getCalendar = async (): Promise<Array<ICalendarObject>> => {
  return await fetch(`${url}/home/calendar`, {
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
    .catch((err) => {
      console.log(err);
    });
};

export { getCalendar };
