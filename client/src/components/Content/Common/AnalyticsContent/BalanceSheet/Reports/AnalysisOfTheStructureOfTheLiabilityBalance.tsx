import React from "react";
import { Table } from "antd";

const AnalysisOfTheStructureOfTheLiabilityBalance = ({
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
      rowSpan: 4,
      align: "center",
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
                      dataIndex: "1",
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
      title: "Собственный капитал",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс.руб",
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
                  title: "сумма, тыс.руб",
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
                  title: "суммы, тыс.руб",
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
      title: "Долгосрочные обязательства",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс.руб",
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
                  title: "сумма, тыс.руб",
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
                  title: "суммы, тыс.руб",
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
      ],
    },
    {
      title: "Краткосрочные обязательства",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс.руб",
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
                  title: "сумма, тыс.руб",
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
                  title: "суммы, тыс.руб",
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
      ],
    },
    {
      title: "в том числе",
      children: [
        {
          title: "краткосрочные кредиты и займы",
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс.руб",
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
                  title: "сумма, тыс.руб",
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
                  title: "суммы, тыс.руб",
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
        {
          title: "краткосрочная кредиторская задолженность",
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс.руб",
                  children: [
                    {
                      title: "26",
                      dataIndex: "26",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "27",
                      dataIndex: "27",
                    },
                  ],
                },
              ],
            },
            {
              title: "На конец отчетного периода",
              children: [
                {
                  title: "сумма, тыс.руб",
                  children: [
                    {
                      title: "28",
                      dataIndex: "28",
                    },
                  ],
                },
                {
                  title: "удельный вес, %",
                  children: [
                    {
                      title: "29",
                      dataIndex: "29",
                    },
                  ],
                },
              ],
            },
            {
              title: "Изменение",
              children: [
                {
                  title: "суммы, тыс.руб",
                  children: [
                    {
                      title: "30",
                      dataIndex: "30",
                    },
                  ],
                },
                {
                  title: "удельного веса, %",
                  children: [
                    {
                      title: "31",
                      dataIndex: "31",
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
      title: "Баланс",
      rowSpan: 2,
      children: [
        {
          rowSpan: 0,
          children: [
            {
              title: "На начало года",
              children: [
                {
                  title: "сумма, тыс.руб",
                  children: [
                    {
                      title: "32",
                      dataIndex: "32",
                    },
                  ],
                },
              ],
            },
            {
              title: "На конец отчетного периода",
              children: [
                {
                  title: "сумма, тыс.руб",
                  children: [
                    {
                      title: "33",
                      dataIndex: "33",
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
      scroll={{ x: true }}
      bordered
      loading={loading}
      rowClassName={rowClassName}
      pagination={false}
    />
  );
};

export default AnalysisOfTheStructureOfTheLiabilityBalance;
