import { Filter } from "../models/common-types";

const tokenName: string = "RepApp";

// development URL
const url: string =
  process.env.NODE_ENV === "development"
    ? "https://10.81.80.100:9999"
    : window.location.protocol === "https:"
    ? "https://10.81.80.100:9999"
    : "http://86.57.172.88:9998";

// production URL
// const url: string =
//   process.env.NODE_ENV === "development"
//     ? "https://10.81.80.100:9999"
//     : window.location.protocol === "https:"
//     ? "/api"
//     : "/api";

const acceptedFiles =
  ".doc,.docx,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document";

const importFiles =
  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel";

const regions: Array<Filter> = [
  { text: "Витебская область", value: "Витебская область" },
  { text: "Минская область", value: "Минская область" },
  { text: "Гомельская область", value: "Гомельская область" },
  { text: "Могилевская область", value: "Могилевская область" },
  { text: "Гродненская область", value: "Гродненская область" },
  { text: "Брестская область", value: "Брестская область" },
  { text: "Минск", value: "Минск" },
];

const yesNo: Array<Filter> = [
  { text: "Нет", value: "Нет" },
  { text: "Да", value: "Да" },
];

const statusReportAdmin: Array<Filter> = [
  { text: "Новый", value: "Новый" },
  { text: "Отправлен на корректировку", value: "Отправлен на корректировку" },
  { text: "После корректировки", value: "После корректировки" },
  { text: "Принят", value: "Принят" },
  { text: "Отозван", value: "Отозван" },
];

const statusReportUser: Array<Filter> = [
  { text: "Новый", value: "Новый" },
  { text: "Отправлен", value: "Отправлен" },
  { text: "Принят", value: "Принят" },
  { text: "Требует корректировки", value: "Требует корректировки" },
  { text: "Отозван", value: "Отозван" },
];

const industrial = [
  { text: "Промышленное", value: true },
  { text: "Непромышленное", value: false },
];

const isIndustrial = ["Непромышленное", "Промышленное"];

const vaultFilter = [
  {
    title: "Всего с Аппаратом Концерна",
    value: 0,
    key: "0",
  },
  {
    title: "Всего без Аппарата Концерна",
    value: 1,
    key: "1",
  },
  {
    title: "Промышленные",
    value: 2,
    key: "2",
    children: [
      {
        title: "Деревообрабатывающие",
        value: 3,
        key: "3",
      },
      {
        title: "ЦБП",
        value: 4,
        key: "4",
      },
      {
        title: "Мебельные",
        value: 5,
        key: "5",
      },
    ],
  },
  {
    title: "Непромышленные",
    value: 6,
    key: "6",
    children: [
      {
        title: "Лесозаготовительные",
        value: 7,
        key: "7",
      },
      {
        title: "Торговые",
        value: 8,
        key: "8",
      },
      {
        title: "Прочие",
        value: 9,
        key: "9",
      },
    ],
  },
  {
    title: "Процент гос. собственности",
    value: 10,
    key: "10",
    disabled: true,
    children: [
      {
        title: "С долей 50% и более",
        value: 11,
        key: "11",
      },
      {
        title: "С долей до 50%",
        value: 12,
        key: "12",
      },
    ],
  },
  {
    title: "Холдинг ДО организаций",
    value: 13,
    key: "13",
  },
];

const analyticsFilter = [
  {
    title: "Промышленные",
    value: 2,
    key: "2",
    children: [
      {
        title: "Деревообрабатывающие",
        value: 3,
        key: "3",
      },
      {
        title: "ЦБП",
        value: 4,
        key: "4",
      },
      {
        title: "Мебельные",
        value: 5,
        key: "5",
      },
    ],
  },
  {
    title: "Непромышленные",
    value: 6,
    key: "6",
    children: [
      {
        title: "Лесозаготовительные",
        value: 7,
        key: "7",
      },
      {
        title: "Торговые",
        value: 8,
        key: "8",
      },
      {
        title: "Прочие",
        value: 9,
        key: "9",
      },
    ],
  },
];

const analyticsReportSelect = [
  { text: "Анализ структуры актива баланса", value: "1" },
  { text: "Анализ структуры пассива баланса", value: "2" },
  { text: "Анализ финансовых показателей", value: "3" },
  { text: "Анализ структуры обязательств", value: "4" },
  { text: "Коэффициенты платежеспособности", value: "5" },
  { text: "Состояние собственных оборотных средств", value: "6" },
];

export {
  tokenName,
  url,
  acceptedFiles,
  importFiles,
  regions,
  yesNo,
  statusReportAdmin,
  statusReportUser,
  industrial,
  isIndustrial,
  vaultFilter,
  analyticsFilter,
  analyticsReportSelect,
};
