import React from "react";
import { Card, Col, Row, Space, Typography, Grid } from "antd";
import { FilePdfOutlined, FileExcelOutlined } from "@ant-design/icons";

const { useBreakpoint } = Grid;

const UserGuidePage = () => {
  const screen = useBreakpoint();
  return (
    <Row gutter={[48, 48]}>
      <Col span={screen.xxl ? 8 : 12}>
        <Card title="Руководство пользователя" bordered={false}>
          <Typography.Link
            style={{ fontSize: 16 }}
            href="user_guide.pdf"
            target="_blank"
          >
            <Space>
              <FilePdfOutlined style={{ color: "#FF1B0E", fontSize: 40 }} />
              Руководство_пользователя.pdf
            </Space>
          </Typography.Link>
        </Card>
      </Col>
      <Col span={screen.xxl ? 8 : 12}>
        <Card title="Файл для импорта данных в бух. баланс" bordered={false}>
          <Typography.Link style={{ fontSize: 16 }} href="report_template.xlsx">
            <Space>
              <FileExcelOutlined style={{ color: "#1D6B41", fontSize: 40 }} />
              Шаблон_Бух_Баланс.xlsx
            </Space>
          </Typography.Link>
        </Card>
      </Col>
    </Row>
  );
};

export default UserGuidePage;
