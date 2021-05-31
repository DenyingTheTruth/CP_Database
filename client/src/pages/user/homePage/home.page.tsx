import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import moment, { Moment } from "moment";

import HomeCalendar from "../../../components/Content/User/HomeCalendar/HomeCalendar";
import HomeTable from "../../../components/Content/User/HomeTable/HomeTable";

import "./home.page.scss";
import { getCalendar } from "../../../services/home/calendar.services";
import { ICalendarObject } from "../../../models/common-interfaces";
import { IHomeTableDataSource } from "../../../models/common-interfaces";

const HomePage = () => {
  const [calendar, setCalendar] = useState<Array<ICalendarObject>>([]);
  const [items, setItems] = useState<Array<IHomeTableDataSource>>([]);
  const [date, setDate] = useState<Moment>(moment(Date.now()));
  const [day, setDay] = useState<Moment | null>(null);
  const [currentMonth, setCurrentMonth] = useState<number | null>(null);

  useEffect(() => {
    (async function updateCalendar() {
      const newCalendar = await getCalendar();
      setCalendar(newCalendar);
    })();
  }, []);

  useEffect(() => {
    setCurrentMonth(moment(date).month());
    const calendarForDataSource: Array<IHomeTableDataSource> = calendar
      ?.filter(
        (item) =>
          moment(item.endDate).month() === moment(date).month() &&
          moment(item.endDate).year() === moment(date).year(),
      )
      .sort((a, b) => moment(a.endDate).unix() - moment(b.endDate).unix())
      .map((item, i) => {
        const { reportTypes } = item.period;
        return reportTypes.map((report) => {
          return {
            key: item.id,
            index: i + 1,
            name: report.name,
            date: `${moment(item.period.startDate).format(
              "DD.MM.YYYY",
            )} - ${moment(item.period.endDate).format("DD.MM.YYYY")}`,
            year: item.year,
          };
        });
      })
      .flat();
    setItems(calendarForDataSource);
  }, [calendar, date]);

  useEffect(() => {
    if (day) {
      const calendarForDataSource: Array<IHomeTableDataSource> = calendar
        ?.filter(
          (item) =>
            moment(item.endDate).format("DD.MM.YYYY") ===
            moment(day).format("DD.MM.YYYY"),
        )
        .sort((a, b) => moment(a.endDate).unix() - moment(b.endDate).unix())
        .map((item, i) => {
          const { reportTypes } = item.period;
          return reportTypes.map((report) => {
            return {
              key: item.id,
              index: i + 1,
              name: report.name,
              date: `${moment(item.period.startDate).format(
                "DD.MM.YYYY",
              )} - ${moment(item.period.endDate).format("DD.MM.YYYY")}`,
              year: item.year,
            };
          });
        })
        .flat();
      setItems(calendarForDataSource);
    }
  }, [day]);

  const changeDate = (data: Moment) => {
    setDate(moment(data));
  };

  const changeDay = (data: Moment) => {
    if (currentMonth === moment(data).month()) {
      setDay(moment(data));
    }
  };

  const columns = [
    {
      title: "№",
      dataIndex: "index",
      key: "index",
    },
    {
      title: "Название отчета",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Отчетный период",
      dataIndex: "date",
      key: "date",
    },
    {
      title: "Действия",
      dataIndex: ["operation"],
      width: 90,
      editable: false,
      render: (_: any, record: any) => {
        return (
          <button className={"action-button-link"}>
            <Link
              to={`/create-balance-sheet?year=${record.year}&id=${record.key}`}
            >
              Создать
            </Link>
          </button>
        );
      },
    },
  ];

  return (
    <div className={"wrapper"}>
      <HomeCalendar
        calendar={calendar}
        changeDate={changeDate}
        changeDay={changeDay}
      />
      <HomeTable dataSource={items} columns={columns} />
    </div>
  );
};

export default HomePage;
