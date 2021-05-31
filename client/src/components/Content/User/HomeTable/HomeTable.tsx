import React from "react";
import { Table } from "antd";

import { IHomeTableDataSource } from "../../../../models/common-interfaces";

const HomeTable = ({
  dataSource,
  columns,
}: {
  dataSource: Array<IHomeTableDataSource>;
  columns: any;
}) => {
  return (
    <>
      <Table
        bordered
        dataSource={dataSource}
        columns={columns}
        rowClassName="editable-row"
      />
    </>
  );
};

export default HomeTable;
