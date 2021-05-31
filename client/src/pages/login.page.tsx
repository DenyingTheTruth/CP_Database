import React from "react";
import { Switch, Route, Redirect } from "react-router-dom";
import LoginForm from "../components/Content/LoginForm";
import "./login.scss";
import "../logo.png";
import logo from "../logo.png";

const LoginPage = () => {
  return (
    <Switch>
      <Route exact path="/login">
        <div className="login-form flex-row --row-center --flex-col">
          <div className={"logo-container flex-row --row-center"}>
            {/*<img src={logo} alt="Bellesbumprom" />*/}
              <h1 style={{color: "#773c1a",  fontSize: 34, margin: 0}}>ClappKeep</h1>
          </div>
          <LoginForm />
        </div>
      </Route>
      <Redirect to={"/login"} />
    </Switch>
  );
};

export default LoginPage;
