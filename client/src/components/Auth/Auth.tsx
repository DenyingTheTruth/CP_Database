import React, {
  useState,
  useEffect,
  createContext,
  useContext,
  useCallback,
} from "react";
import { useHistory } from "react-router-dom";
import { tokenName } from "../../constants/constants";
import { token, setCookie } from "../../helpers/helpers";
import { loginRequest, tokenExist } from "../../services/auth/auth.services";
import { IContext } from "../../models/common-interfaces";
import moment, { Moment } from "moment";

const AuthContext = createContext<IContext>({ user: null, loading: true });
const useAuth = () => useContext(AuthContext);

function AuthProvider(props: any) {
  const [user, setUser] = useState<object | null>(null);
  const [loading, setLoading] = useState(true);
  const [gYear, setGYear] = useState<Moment | null>(moment(new Date()));
  const [err, setErr] = useState("");
  const [reportsRevision, setReportsRevision] = useState<number | null>(null);
  const [unreadReportsCount, setUnreadReportsCount] = useState<number | null>(null);
  const history = useHistory();

  const logIn = useCallback(async (username, password) => {
    const { user, status } = await loginRequest(username, password);
    if (status) {
      setErr("Неправильное имя пользователя или пароль");
    } else {
      setUser(user);
      setReportsRevision(user.revisionReports);
      setUnreadReportsCount(user.unreadReportsCount);
      history.push("/");
    }
  }, []);

  const logOut = useCallback(() => {
    setCookie(tokenName, "", {
      "max-age": -1,
    });
    setUser(null);
  }, []);

  const gYearChange = (value: Moment | null) => {
    setGYear(value);
  };

  useEffect(() => {
    if (token()) {
      const getUser = async () => {
        const { user } = await tokenExist(token(), history);
        setUser(user);
        setReportsRevision(user?.revisionReports);
        setUnreadReportsCount(user?.unreadReportsCount);
        setLoading(false);
      };
      getUser();
    } else setLoading(false);
  }, [history]);

  const changeReportsRevision = (value: number) => {
    setReportsRevision(value);
  };

  const changeUnreadReports = (value: number) => {
    setUnreadReportsCount(value);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        logIn,
        logOut,
        loading,
        err,
        reportsRevision,
        unreadReportsCount,
        gYear,
        gYearChange,
        changeReportsRevision,
        changeUnreadReports,
      }}
      {...props}
    />
  );
}

export { AuthProvider, useAuth, AuthContext };
