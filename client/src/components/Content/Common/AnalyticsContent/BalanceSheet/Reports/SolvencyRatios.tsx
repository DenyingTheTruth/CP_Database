import React from "react";
import { Table } from "antd";

const SolvencyRatios = ({
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
      title: "Название",
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
      title: "Коэффициент текущей ликвидности",
      children: [
        {
          title: "нормативный коэффициент",
          children: [
            {
              title: "2",
              dataIndex: "2",
            },
          ],
        },
        {
          title: "На начало года",
          children: [
            {
              title: "3",
              dataIndex: "3",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "4",
              dataIndex: "4",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "5",
              dataIndex: "5",
            },
          ],
        },
      ],
    },
    {
      title: "Коэффициент обеспеченности собственными оборотными средствами",
      children: [
        {
          title: "нормативный коэффициент",
          children: [
            {
              title: "6",
              dataIndex: "6",
            },
          ],
        },
        {
          title: "На начало года",
          children: [
            {
              title: "7",
              dataIndex: "7",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "8",
              dataIndex: "8",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "9",
              dataIndex: "9",
            },
          ],
        },
      ],
    },
    {
      title: "Коэффициент обеспеченности финансовых обязательств активами",
      children: [
        {
          title: "На начало года",
          children: [
            {
              title: "10",
              dataIndex: "10",
            },
          ],
        },
        {
          title: "На конец отчетного периода",
          children: [
            {
              title: "11",
              dataIndex: "11",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "12",
              dataIndex: "12",
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

export default SolvencyRatios;
