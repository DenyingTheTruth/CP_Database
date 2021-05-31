import React from "react";
import { Table } from "antd";

const AnalysisOfFinancialIndicators = ({
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
      align: "center",
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
      title: "Выручка от реализации продукции, товаров, работ, услуг, тыс.руб.",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "2",
              dataIndex: "2",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
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
      title: "Полная себестоимость реализованной продукции, тыс.руб.",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "5",
              dataIndex: "5",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
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
      title: "Затраты на рубль реализованной продукции, тыс.руб.",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "8",
              dataIndex: "8",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
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
      title: "Прибыль от реализации продукции, тыс.руб.",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "11",
              dataIndex: "11",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
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
      title: "Прибыль до налогообложения, тыс.руб.",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "14",
              dataIndex: "14",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
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
      title: "Чистая прибыль, тыс.руб.",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "17",
              dataIndex: "17",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
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
    {
      title: "Рентабельность продаж, %",
      children: [
        {
          title: "за отчетный период",
          children: [
            {
              title: "20",
              dataIndex: "20",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
          children: [
            {
              title: "21",
              dataIndex: "21",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "22",
              dataIndex: "22",
            },
          ],
        },
      ],
    },
    {
      title: "Рентабельность продукции (работ, услуг), %",
      children: [
        {
          title: "за отчетный период,",
          children: [
            {
              title: "23",
              dataIndex: "23",
            },
          ],
        },
        {
          title: "за аналогичный период прошлого года",
          children: [
            {
              title: "24",
              dataIndex: "24",
            },
          ],
        },
        {
          title: "Изменение",
          children: [
            {
              title: "25",
              dataIndex: "25",
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

export default AnalysisOfFinancialIndicators;
