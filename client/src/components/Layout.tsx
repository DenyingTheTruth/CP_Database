import React from "react";

// additional files
import "../App.less";
import "../app.scss";
import "./Layout.scss";

// antd
import { Layout as AntLayout } from "antd";

// auth
import { useAuth } from "./Auth/Auth";

// custom components
import UserMenu from "./Sider/User/UserMenu";
import AdminMenu from "./Sider/Admin/AdminMenu";
import Header from "./Header/Header";
import Footer from "./Footer/Footer";

// routes
import UserRoutes from "../routes/user.routes";
import AdminRoutes from "../routes/admin.routes";

const Layout = ({ isAdmin }: { isAdmin: boolean }) => {
  const { user, logOut } = useAuth();

  return (
    <AntLayout id="entirePage">
      <Header user={user} logOut={logOut} />
      <AntLayout className="site-layout content-sider-wrap">
        {isAdmin ? <AdminMenu /> : <UserMenu />}
        <AntLayout>
          <AntLayout.Content id="layoutContent" className="layout-content-wrap">
            {isAdmin ? <AdminRoutes /> : <UserRoutes />}
          </AntLayout.Content>
          {/*<Footer />*/}
        </AntLayout>
      </AntLayout>
    </AntLayout>
  );
};

export default Layout;
