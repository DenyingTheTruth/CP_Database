import React from "react";
import { Table } from "antd";

const AnalysisOfTheBalanceSheetAssetStructure = ({
  data,
  tab,
  loading,
  isAdmin,
}: {
  data: Array<any> | undefined;
  tab: number;
  loading: boolean;
  isAdmin: boolean;
}) => {
  const columns: any = [
    {
      title: isAdmin ? "Наименование организаций" : "Наименование организации",
      align: "center",
      rowSpan: 4,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              rowSpan: 0,
              children: [
                {
                  rowSpan: 0,
                  children: [
                    {
                      title: "1",
                      dataIndex: 1,
                    },
                  ],
                },
              ],
            },
          ],
        },
      ],
    },
    {
      title: tab === 1 ? "Долгосрочные активы" : "Краткосрочные активы",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "2",
                      dataIndex: "2",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "3",
                      dataIndex: "3",
                    },
                  ],
                },
              ],
            },
            {
              title: "На конец отчетного периода",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "4",
                      dataIndex: "4",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
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
              title: "Изменение",
              children: [
                {
                  title: "суммы, тыс. руб.",
                  children: [
                    {
                      title: "6",
                      dataIndex: "6",
                    },
                  ],
                },
                {
                  title: "удельного веса, %",
                  children: [
                    {
                      title: "7",
                      dataIndex: "7",
                    },
                  ],
                },
              ],
            },
          ],
        },
      ],
    },
    {
      title: "В том числе",
      colSpan: 18,
      children: [
        {
          title: tab === 1 ? "основные средства" : "запасы",
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "8",
                      dataIndex: "8",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
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
              title: "На конец отчетного периода",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "10",
                      dataIndex: "10",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "11",
                      dataIndex: "11",
                    },
                  ],
                },
              ],
            },
            {
              title: "Изменение",
              children: [
                {
                  title: "суммы, тыс. руб.",
                  children: [
                    {
                      title: "12",
                      dataIndex: "12",
                    },
                  ],
                },
                {
                  title: "удельного веса, %",
                  children: [
                    {
                      title: "13",
                      dataIndex: "13",
                    },
                  ],
                },
              ],
            },
          ],
        },
        {
          title:
            tab === 1
              ? "долгосрочные финансовые вложения"
              : "краткосрочная дебиторская задолженность",
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "14",
                      dataIndex: "14",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "15",
                      dataIndex: "15",
                    },
                  ],
                },
              ],
            },
            {
              title: "На конец отчетного периода",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "16",
                      dataIndex: "16",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "17",
                      dataIndex: "17",
                    },
                  ],
                },
              ],
            },
            {
              title: "Изменение",
              children: [
                {
                  title: "суммы, тыс. руб.",
                  children: [
                    {
                      title: "18",
                      dataIndex: "18",
                    },
                  ],
                },
                {
                  title: "удельного веса, %",
                  children: [
                    {
                      title: "19",
                      dataIndex: "19",
                    },
                  ],
                },
              ],
            },
          ],
        },
        {
          title:
            tab === 1
              ? "долгосрочная дебиторская задолженность"
              : "денежные средства и эквиваленты",
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "20",
                      dataIndex: "20",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "21",
                      dataIndex: "21",
                    },
                  ],
                },
              ],
            },
            {
              title: "На конец отчетного периода",
              children: [
                {
                  title: "сумма, тыс. руб.",
                  children: [
                    {
                      title: "22",
                      dataIndex: "22",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "23",
                      dataIndex: "23",
                    },
                  ],
                },
              ],
            },
            {
              title: "Изменение",
              children: [
                {
                  title: "суммы, тыс. руб.",
                  children: [
                    {
                      title: "24",
                      dataIndex: "24",
                    },
                  ],
                },
                {
                  title: "удельного веса, %",
                  children: [
                    {
                      title: "25",
                      dataIndex: "25",
                    },
                  ],
                },
              ],
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
      bordered
      scroll={{ x: true }}
      loading={loading}
      rowClassName={rowClassName}
      pagination={false}
    />
  );
};

export default AnalysisOfTheBalanceSheetAssetStructure;
