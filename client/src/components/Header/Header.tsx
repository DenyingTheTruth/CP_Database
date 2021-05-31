import React from "react";
import { Avatar, DatePicker, Dropdown, Layout, Menu } from "antd";
import { DownOutlined, LogoutOutlined, UserOutlined } from "@ant-design/icons";
import logo from "../../logo.png";

import { User } from "../../models/common-types";
import { Moment } from "moment";
import { useAuth } from "../Auth/Auth";

const Header = ({ user, logOut }: { user: User; logOut: () => void }) => {
  const { gYearChange, gYear } = useAuth();

  const onGYearChange = (value: Moment | null) => {
    gYearChange(value);
  };

  const menu = (
    <Menu>
      <Menu.Item>
        <span
          onClick={() => logOut()}
          className={"flex-row --row-start span-link"}
        >
          <LogoutOutlined style={{ fontSize: "20px", color: "#773c1a" }} />
          <span>Выйти</span>
        </span>
      </Menu.Item>
    </Menu>
  );

  const organizationName: string = user?.organization?.name || "Администратор";

  return (
    <Layout.Header className="header">
      <div></div>
      <div className={"logo-container flex-row --row-center"}>
        {/*<img src={logo} alt="Bellesbumprom" />*/}
        <h1 style={{color: "#773c1a", fontSize: 34, margin: 0}}>ClappKeep</h1>
      </div>
      <div className="flex-row">
        <div>
          {user?.role === "Admin" ? (
            <DatePicker
              style={{ marginRight: "25px" }}
              value={gYear}
              picker="year"
              onChange={onGYearChange}
            />
          ) : (
            ""
          )}
        </div>
        <Dropdown
          className={"flex-row --row-center"}
          overlayStyle={{ width: "fit-content" }}
          overlay={menu}
        >
          <span className="ant-dropdown-link flex-row bold-row height-100">
            <Avatar
              style={{ color: "#773c1a", marginRight: "10px" }}
              size={"small"}
              icon={<UserOutlined />}
            />
            <span className={"profile-dd-text"} style={{ padding: "0 10px" }}>
              {organizationName || "Админ"}
            </span>
            <DownOutlined
              style={{ fontSize: "14px", color: "#773c1a", marginTop: "3px" }}
            />
          </span>
        </Dropdown>
      </div>
    </Layout.Header>
  );
};

export default Header;
