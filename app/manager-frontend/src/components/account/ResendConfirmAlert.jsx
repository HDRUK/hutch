import { Link } from "@chakra-ui/react";
import { TitledAlert } from "components/TitledAlert";
import { useTranslation } from "react-i18next";
import { Link as RouterLink } from "react-router-dom";

export const ResendConfirmAlert = ({ userIdOrEmail }) => {
  const { t } = useTranslation();
  return (
    <TitledAlert title={t("feedback.account.resendConfirm_title")}>
      <Link
        as={RouterLink}
        to={`/account/confirm/resend`}
        state={{ userIdOrEmail }}
      >
        {t("feedback.account.resendConfirm_link")}
      </Link>
    </TitledAlert>
  );
};
