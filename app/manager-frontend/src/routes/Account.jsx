import { Confirm } from "pages/account/Confirm";
import { Login } from "pages/account/Login";
import { Register } from "pages/account/Register";
import { ResendConfirm } from "pages/account/ResendConfirm";
import { ResetPassword } from "pages/account/ResetPassword";
import { NotFound } from "pages/error/NotFound";
import { Route, Routes } from "react-router-dom";
import { ActivateAccount } from "pages/account/ActivateAccount";

export const Account = () => (
  <Routes>
    <Route path="login" element={<Login />} />
    <Route path="register" element={<Register />} />
    <Route path="confirm" element={<Confirm />} />
    <Route path="confirm/resend" element={<ResendConfirm />} />
    <Route path="password" element={<ResetPassword />} />
    <Route path="activate" element={<ActivateAccount />} />

    <Route path="*" element={<NotFound />} />
  </Routes>
);
