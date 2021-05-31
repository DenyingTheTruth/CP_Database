import React, { useState, useEffect, useContext } from "react";
import { Menu, Layout as AntLayout, Button } from "antd";
import {
  SnippetsOutlined,
  TeamOutlined,
  ClusterOutlined,
  AuditOutlined,
  BarChartOutlined,
  ReconciliationOutlined,
  FundOutlined,
  CalendarOutlined,
  SettingOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  ProfileOutlined,
  QuestionCircleOutlined,
} from "@ant-design/icons";
import { Link, useLocation } from "react-router-dom";
import { useAuth } from "../../Auth/Auth";

const { SubMenu, Item } = Menu;

const AdminMenu = () => {
  const [menuOpened, setMenuOpened] = useState<boolean>(true);

  const location = useLocation();
  const { unreadReportsCount } = useAuth();

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
        "flex: 0 0 265px; max-width: 265px; min-width: 265px; width: 265px;",
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
        width={265}
        theme={"light"}
        className="fixed-sider"
        collapsed={!menuOpened}
      >
        <Menu
          style={{ marginTop: "10px", paddingBottom: "20px" }}
          theme="light"
          mode="inline"
          selectedKeys={[location.pathname]}
        >
          <Item
            key="/"
            icon={<SnippetsOutlined className="icon-profile-outlined"/>}
            style={{ display: "flex" }}
          >
            <span className="correct-count-wrap">
              <Link to="/">Отчётность организаций</Link>
              {unreadReportsCount ? (
                  <span
                      style={{ marginLeft: "10px" }}
                      className="report-count-mini"
                  >
                    {unreadReportsCount}
                  </span>
              ) : null}
            </span>
          </Item>
          <SubMenu
            key="sub1"
            icon={<BarChartOutlined style={{ fontSize: "20px" }} />}
            title={
              <span>
                <span>Статистика</span>
              </span>
            }
          >
            <Item
              key="/accepted-reports"
              icon={<AuditOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/accepted-reports">Принятые отчеты</Link>
            </Item>
            <Item
              key="/vault-concern"
              icon={<ReconciliationOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/vault-concern">Своды по Концерну</Link>
            </Item>
          </SubMenu>
          <SubMenu
            key="sub3"
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
          <SubMenu
            key="sub2"
            icon={<SettingOutlined style={{ fontSize: "20px" }} />}
            title={
              <span>
                <span>Настройки</span>
              </span>
            }
          >
            <Item
              key="/organizations"
              icon={<ClusterOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/organizations">Организации</Link>
            </Item>
            <Item
              key="/users"
              icon={<TeamOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/users">Пользователи</Link>
            </Item>
            <Item
              key="/reporting-deadlines"
              icon={<CalendarOutlined style={{ fontSize: "20px" }} />}
            >
              <Link to="/reporting-deadlines">Сроки сдачи отчетности</Link>
            </Item>
          </SubMenu>
          <Item
            key="/admin-guide"
            icon={<QuestionCircleOutlined style={{ fontSize: "20px" }} />}
          >
            <Link to="/admin-guide">Руководство администратора</Link>
          </Item>
        </Menu>
      </AntLayout.Sider>
    </>
  );
};

export default AdminMenu;
