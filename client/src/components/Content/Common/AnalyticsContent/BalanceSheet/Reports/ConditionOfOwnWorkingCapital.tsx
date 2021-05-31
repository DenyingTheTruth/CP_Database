import React from "react";
import { Table } from "antd";

const ConditionOfOwnWorkingCapital = ({
  data,
  loading,
  isAdmin,
}: {
  data: Array<any> | undefined;
  loading: boolean;
  isAdmin: boolean;
}) => {
  const columns: any = [
    {
      title: isAdmin ? "Наименование организаций" : "Наименование организации",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "1",
              dataIndex: "1",
              width: 150,
            },
          ],
        },
      ],
    },
    {
      title: "Нормативный коэффициент",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "2",
              dataIndex: "2",
              width: 100,
            },
          ],
        },
      ],
    },
    {
      title: "Собственные оборотные средства, тыс.руб.",
      children: [
        {
          title: "Фактическое наличие",
          children: [
            {
              title: "3",
              dataIndex: "3",
              width: 100,
            },
          ],
        },
        {
          title: "Норматив",
          children: [
            {
              title: "4",
              dataIndex: "4",
              width: 100,
            },
          ],
        },
        {
          title: "Недостаток",
          children: [
            {
              title: "5",
              dataIndex: "5",
              width: 100,
            },
          ],
        },
        {
          title: "Свободный остаток",
          children: [
            {
              title: "6",
              dataIndex: "6",
              width: 100,
            },
          ],
        },
      ],
    },
  ];

  const rowClassName = (record: any) => {
    return Object.keys(record).includes("isBold") ? "bold" : "";
  };

  return (
    <Table
      className="analytics-table"
      dataSource={data}
      columns={columns}
      rowKey={"1"}
      scroll={{ x: "max-content" }}
      bordered
      loading={loading}
      rowClassName={rowClassName}
      pagination={false}
    />
  );
};

export default ConditionOfOwnWorkingCapital;
