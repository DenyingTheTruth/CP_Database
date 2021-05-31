import React from "react";

import { Layout, Space, Typography } from "antd";
import { PhoneOutlined, MailOutlined } from "@ant-design/icons";

const Footer = () => {
  return (
    <Layout.Footer style={{ textAlign: "center" }}>
      <Space>
        <Typography.Text>
          Разработано ООО "Каспел-АН" (
          <Typography.Link
            href="https://caspel.by/"
            style={{ fontWeight: "bold" }}
          >
            caspel.by
          </Typography.Link>
          ).
        </Typography.Text>
        <Typography.Text>
          <Space>
            По вопросам сотрудничества обращаться по следующим контактам:
            <Typography.Text>
              <Space>
                <PhoneOutlined style={{ color: "#773c1a" }} />
                <Typography.Link
                  style={{ whiteSpace: "nowrap" }}
                  href="tel:+375173175261"
                >
                  +375-17-317-52-61
                </Typography.Link>
              </Space>
            </Typography.Text>
            <Typography.Text>
              <Space>
                <MailOutlined style={{ color: "#773c1a" }} />
                <Typography.Link
                  style={{ whiteSpace: "nowrap" }}
                  href="mailto:info@caspel.by"
                >
                  info@caspel.by
                </Typography.Link>
              </Space>
            </Typography.Text>
          </Space>
        </Typography.Text>
      </Space>
    </Layout.Footer>
  );
};

export default Footer;
