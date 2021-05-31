import { Moment } from "moment";

type TypeActivity = {
  id: string;
  name: string;
  isIndustrial: boolean;
};

type Organization = {
  name: string;
  description: string;
  organization: string;
  isHolding: boolean;
  isState: boolean;
  region: number | null;
  typeActivityId: string | null;
  typeActivity: TypeActivity;
  id: string;
};

type User = {
  id: string;
  role: string;
  organization: Organization | null;
  login: string;
  revisionReports: number | null;
  unreadReportsCount: number | null;
} | null;

type OrganizationUser = {
  id: string;
  email: string;
  userName: string;
  organization: Organization | null;
  password: string | null;
  fio: string | null;
};

type NestedPeriod = {
  id: string;
  name: string;
};

type ReportType = {
  periods: Array<NestedPeriod>;
  name: string;
  id: string;
};

type NestedReportType = {
  id: string;
  name: string;
};

type Period = {
  reportTypes: Array<NestedReportType>;
  name: string;
  id: string;
  isFree?: boolean;
  userCheckinIntervalId: string;
  startDate: string;
  endDate: string;
};

type IntervalPeriod = {
  periodId: string;
  name: string;
  date: Moment | string | null;
};

type Filter = {
  text: string;
  value: string;
};

type GeneralInformation = {
  id: string | undefined;
  UNP: string | undefined;
  TypeEconomicActivity: string | undefined;
  OrganizationalLegalForm: string | undefined;
  GovermentForReport: string | undefined;
  UnitForReport: string | undefined;
  Address: string | undefined;
  Position1: string | undefined;
  FullName1: string | undefined;
  Position2: string | undefined;
  FullName2: string | undefined;
};

export type {
  TypeActivity,
  Organization,
  User,
  ReportType,
  Period,
  OrganizationUser,
  IntervalPeriod,
  Filter,
  NestedPeriod,
  GeneralInformation,
};
