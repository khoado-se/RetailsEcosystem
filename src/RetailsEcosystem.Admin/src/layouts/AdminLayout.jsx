import { Outlet } from "react-router";
import Header from "./Header";
import Footer from "./Footer";
import Sidebar from "./Sidebar";
import Main from "./Main";

export default function AdminLayout() {
  return (
    <>
      <Header />
      <div className="container-fluid">
        <div className="row">
          <Sidebar />
          <Main>
            <Outlet />
          </Main>
        </div>
      </div>
      {/* <Footer /> */}
    </>
  );
}
