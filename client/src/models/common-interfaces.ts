import { Period, User, Organization, IntervalPeriod } from "./common-types";

interface IContext {
  user: User;
  loading: boolean;
  [propName: string]: any;
}

interface ICalendarObject {
  isOverdue: boolean;
  reportCreated: boolean;
  role: string;
  year: number;
  periodId: string;
  period: Period;
  date: string;
  endDate: string;
  reportId: string | null;
  id: string;
}

interface IHomeTableDataSource {
  key: string;
  index: number;
  name: string;
  date: string;
}

interface IIntervals {
  organizationId: string;
  organization: Organization;
  periods: Array<IntervalPeriod>;
  isFull: boolean;
}

interface IOrgForCopy {
  organizationId: string;
  organizationName: string;
}

interface IUserReportTable {
  index: number;
  id: string;
  name: string;
  sentDate?: string;
  status: string;
  note: string;
  date: string;
  year: number;
  period: string;
}

interface IUserReportHistory {
  index: number;
  id: string;
  name: string;
  date: string;
  status: string;
  note: string;
  year: number;
  period: string;
  reportId: string;
}

export type {
  IContext,
  ICalendarObject,
  IHomeTableDataSource,
  IIntervals,
  IOrgForCopy,
  IUserReportTable,
  IUserReportHistory,
};
