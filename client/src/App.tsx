import React from "react";

// router
import { BrowserRouter as Router } from "react-router-dom";

// auth
import { useAuth, AuthProvider } from "./components/Auth/Auth";

// antd
import { Spin, ConfigProvider, Grid } from "antd";
import { WarningOutlined } from "@ant-design/icons";
import ruRU from "antd/lib/locale/ru_RU";

// custom components
import LoginPage from "./pages/login.page";
import Layout from "./components/Layout";

// moment locale
import moment from "moment";
import "moment/locale/ru";
moment.locale("ru");

const { useBreakpoint } = Grid;

function App() {
  const { user, loading } = useAuth();

  const screen = useBreakpoint();

  if (
    Object.keys(screen).length !== 0 &&
    screen.constructor === Object &&
    !screen.lg
  ) {
    return (
      <div className="loading-page flex-row --row-center --flex-col">
        <WarningOutlined style={{ color: "#773C1A", fontSize: 75 }} />
        <span style={{ width: "90%", fontSize: 24, textAlign: "center" }}>
          Вход с мобильных устройств не поддерживается
        </span>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="loading-page flex-row --row-center">
        <Spin size="large" />
      </div>
    );
  }

  if (user) {
    return <Layout isAdmin={user.role === "Admin"} />;
  }

  return <LoginPage />;
}

export default function AppWrap() {
  return (
    <Router>
      <AuthProvider>
        <div className={`app`}>
          <ConfigProvider locale={ruRU}>
            <App />
          </ConfigProvider>
        </div>
      </AuthProvider>
    </Router>
  );
}
