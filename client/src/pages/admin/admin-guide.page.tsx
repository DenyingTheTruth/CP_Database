import React from "react";
import { Card, Col, Row, Space, Typography, Grid } from "antd";
import { FilePdfOutlined } from "@ant-design/icons";

const { useBreakpoint } = Grid;

const AdminGuidePage = () => {
  const screen = useBreakpoint();
  return (
    <Row gutter={[48, 48]}>
      <Col span={screen.xxl ? 8 : 12}>
        <Card title="Руководство администратора" bordered={false}>
          <Typography.Link
            style={{ fontSize: 16 }}
            href="admin_guide.pdf"
            target="_blank"
          >
            <Space>
              <FilePdfOutlined style={{ color: "#FF1B0E", fontSize: 40 }} />
              Руководство_администратора.pdf
            </Space>
          </Typography.Link>
        </Card>
      </Col>
    </Row>
  );
};

export default AdminGuidePage;
