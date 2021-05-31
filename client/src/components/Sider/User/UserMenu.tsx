import React, { useContext, useState, useEffect } from "react";
import { Menu, Layout as AntLayout, Button } from "antd";
import {
  HomeOutlined,
  HistoryOutlined,
  FileAddOutlined,
  ProfileOutlined,
  QuestionCircleOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  OrderedListOutlined,
  FundOutlined,
} from "@ant-design/icons";
import { Link, useLocation } from "react-router-dom";

import { AuthContext } from "../../Auth/Auth";

import "./UserMenu.scss";

const { SubMenu, Item } = Menu;

const UserMenu = () => {
  const location = useLocation();
  const reportsRevision = useContext(AuthContext).reportsRevision;

  const [menuOpened, setMenuOpened] = useState<boolean>(true);

  useEffect(() => {
    const menuButton = document.querySelector("#menuToggleButton");
    menuButton?.addEventListener("click", (e) => {
      if (menuButton.classList.contains("closed")) {
        setMenuOpened(true);
      } else {
        setMenuOpened(false);
      }
    });
  }, []);

  useEffect(() => {
    const entirePage = document.querySelector("#entirePage");
    const menuButton = document.querySelector("#menuToggleButton");
    const layoutPage = document.querySelector(
      ".site-layout.content-sider-wrap",
    );
    const layoutContent = document.querySelector("#layoutContent");
    const sider = document.querySelector(".fixed-sider");
    const menu = document.querySelector(".ant-menu.ant-menu-root");
    if (!menuOpened) {
      entirePage?.classList.add("open-menu");
      menuButton?.classList.add("closed");
      layoutPage?.setAttribute("style", "margin-left: 80px; transition: .2s;");
      layoutContent?.classList.remove("--menu-opened");
      sider?.setAttribute(
        "style",
        "flex: 0 0 80px; max-width: 80px; min-width: 80px; width: 80px;",
      );
      sider?.classList.add("ant-layout-sider-collapsed");
      menu?.classList.add("ant-menu-inline-collapsed");
    } else {
      entirePage?.classList.add("open-menu");
      menuButton?.classList.remove("closed");
      layoutPage?.setAttribute("style", "margin-left: 260px; transition: .2s;");
      layoutContent?.classList.add("--menu-opened");
      sider?.setAttribute(
        "style",
        "flex: 0 0 260px; max-width: 260px; min-width: 260px; width: 260px;",
      );
      sider?.classList.remove("ant-layout-sider-collapsed");
      menu?.classList.remove("ant-menu-inline-collapsed");
    }
  }, [menuOpened]);

  return (
    <>
      <Button
        type="text"
        className={"flex-row --row-start menu-toggle-button"}
        style={{ padding: "5px 10px" }}
        id="menuToggleButton"
      >
        {!menuOpened ? (
          <MenuFoldOutlined style={{ fontSize: "20px", color: "#773c1a" }} />
        ) : (
          <MenuUnfoldOutlined style={{ fontSize: "20px", color: "#773c1a" }} />
        )}
      </Button>
      <AntLayout.Sider
        width={260}
        theme={"light"}
        className="fixed-sider"
        collapsed={!menuOpened}
      >
        <Menu
          style={{ marginTop: "20px" }}
          theme="light"
          mode="inline"
          selectedKeys={[location.pathname]}
          // defaultOpenKeys={["sub1", "sub2"]}
        >
          <Item key="/" icon={<HomeOutlined style={{ fontSize: "20px" }} />}>
            <Link to="/">Главная</Link>
          </Item>
          <Item
            key="/general-information"
            icon={<OrderedListOutlined style={{ fontSize: "20px" }} />}
          >
            <Link to="/general-information">Общие сведения</Link>
          </Item>
          <Item
            key="/history-send-reports"
            icon={<HistoryOutlined style={{ fontSize: "20px" }} />}
          >
            <Link to="/history-send-reports">История подачи отчетности</Link>
          </Item>
          <SubMenu
            key="sub1"
            icon={<FileAddOutlined style={{ fontSize: "20px" }} />}
            title={<span>Создание отчета</span>}
          >
            <Item
              key="/create-balance-sheet"
              icon={<ProfileOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/create-balance-sheet">
                Бухгалтерский баланс и приложения к нему
              </Link>
            </Item>
          </SubMenu>
          <Item
            key="/correct-reports"
            icon={<ProfileOutlined className="icon-profile-outlined" />}
            style={{ display: "flex" }}
          >
            <span className="correct-count-wrap">
              <Link to="/correct-reports">Корректировка</Link>
              {reportsRevision ? (
                <span
                  style={{ marginLeft: "10px" }}
                  className="report-count-mini"
                >
                  {reportsRevision}
                </span>
              ) : null}
            </span>
          </Item>
          <SubMenu
            key="sub2"
            icon={<FundOutlined style={{ fontSize: "20px" }} />}
            title={
              <span>
                <span>Аналитика</span>
              </span>
            }
          >
            <Item
              key="/analytics/balance-sheet"
              icon={<ProfileOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/analytics/balance-sheet">
                Бухгалтерский баланс и приложения к нему
              </Link>
            </Item>
          </SubMenu>
          <Item
            key="/user-guide"
            icon={<QuestionCircleOutlined style={{ fontSize: "20px" }} />}
          >
            <Link to="/user-guide">Руководство пользователя</Link>
          </Item>
        </Menu>
      </AntLayout.Sider>
    </>
  );
};

export default UserMenu;
