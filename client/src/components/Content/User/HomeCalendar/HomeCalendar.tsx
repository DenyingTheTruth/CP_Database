import React, { ReactNode } from "react";

import { Calendar } from "antd";

import moment, { Moment } from "moment";

import { ICalendarObject } from "../../../../models/common-interfaces";

import "./HomeCalendar.scss";

const HomeCalendar = ({
  calendar,
  changeDate,
  changeDay,
}: {
  calendar: Array<ICalendarObject>;
  changeDate: (data: Moment) => void;
  changeDay: (data: Moment) => void;
}) => {
  const dateCellRender = (date: Moment): ReactNode => {
    if (calendar?.find((item) => moment(item.endDate).isSame(date, "day"))) {
      return (
        <div className={"cell__report"}>
          <span className={"report-count"}>1</span>
        </div>
      );
    }
  };

  const onPanelChange = (value: Moment) => {
    changeDate(value);
  };

  const onChange = (value: Moment) => {
    changeDay(value);
  };

  return (
    <Calendar
      dateCellRender={dateCellRender}
      className={"home__calendar"}
      onPanelChange={onPanelChange}
      onChange={onChange}
      fullscreen
    />
  );
};

export default HomeCalendar;
