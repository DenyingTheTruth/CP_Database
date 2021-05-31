import React from "react";
import { Table } from "antd";

// АНАЛИЗ СТРУКТУРЫ ОБЯЗАТЕЛЬСТВ

const AnalysisOfTheStructureOfLiabilities = ({
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
            },
          ],
        },
      ],
    },
    {
      title: "Собственный капитал, тыс.руб.",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "2",
              dataIndex: "2",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "3",
              dataIndex: "3",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "4",
              dataIndex: "4",
            },
          ],
        },
      ],
    },
    {
      title: "Заемный капитал, тыс.руб.",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "5",
              dataIndex: "5",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "6",
              dataIndex: "6",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "7",
              dataIndex: "7",
            },
          ],
        },
      ],
    },
    {
      title: "Общая сумма капитала",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "8",
              dataIndex: "8",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "9",
              dataIndex: "9",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "10",
              dataIndex: "10",
            },
          ],
        },
      ],
    },
    {
      title: "Коэффициент финансовой независимости",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "11",
              dataIndex: "11",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "12",
              dataIndex: "12",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "13",
              dataIndex: "13",
            },
          ],
        },
      ],
    },
    {
      title: "Коэффициент финансовой напряженности",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "14",
              dataIndex: "14",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "15",
              dataIndex: "15",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "16",
              dataIndex: "16",
            },
          ],
        },
      ],
    },
    {
      title: "Коэффициент капитализации (финансового риска)",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "17",
              dataIndex: "17",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "18",
              dataIndex: "18",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "19",
              dataIndex: "19",
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
      scroll={{ x: true }}
      bordered
      loading={loading}
      rowClassName={rowClassName}
      pagination={false}
    />
  );
};

export default AnalysisOfTheStructureOfLiabilities;
