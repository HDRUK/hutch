import { Container, Text } from "@chakra-ui/react";
import { BusyPage } from "components/Busy";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";
import { useQueryStringViewModel } from "helpers/hooks/useQueryStringViewModel";
import { Suspense, useCallback } from "react";
import { useAsync } from "react-async";
import { useTranslation } from "react-i18next";
import { useLocation } from "react-router-dom";

const SuccessFeedback = () => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <TitledAlert
        status="success"
        title={t("resendConfirm.feedback.success_title")}
      >
        <Text>{t("resendConfirm.feedback.success_message")}</Text>
      </TitledAlert>
    </Container>
  );
};

const ErrorFeedback = ({ tKey }) => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <TitledAlert status="error" title={t("feedback.error_title")}>
        <Text>{t(tKey ?? "feedback.sendEmailFailed")}</Text>
      </TitledAlert>
    </Container>
  );
};

const useResendHandler = () => {
  const {
    account: { resendConfirm },
  } = useBackendApi();
  return useCallback(
    async ({ userIdOrEmail }) => {
      await resendConfirm(userIdOrEmail).json(); // returns 204
      return true; // any failure will throw before this point :)
    },
    [resendConfirm]
  );
};

// this actually does the hard work
// but we use suspense at the page level while its busy
const ResendConfirmationLink = () => {
  // So `userId` can be sourced from 2 different places...
  // If we got here via another page in the app
  // (e.g. Login, Confirm)
  // then it'll be in Location state
  // whereas from a user's email link it'll be in the query string viewmodel
  const { userIdOrEmail: stateUserIdOrEmail } = useLocation().state ?? {};
  const { userId: queryUserId } = useQueryStringViewModel();
  const userIdOrEmail = queryUserId ?? stateUserIdOrEmail;
  const handleResend = useResendHandler();

  const { error, data } = useAsync(handleResend, {
    suspense: true,
    userIdOrEmail,
  });

  if (error) {
    let tKey;
    switch (error?.response?.status) {
      case 400:
      case 404:
        tKey = "feedback.account.invalidResendLink";
        break;
    }

    console.error(error);
    return <ErrorFeedback tKey={tKey} />;
  }

  if (data) {
    return <SuccessFeedback />;
  }

  return null;
};

export const ResendConfirm = () => (
  <Suspense
    fallback={
      <BusyPage
        tKey="resendConfirm.feedback.busy"
        containerProps={{ justifyContent: "center" }}
      />
    }
  >
    <ResendConfirmationLink />
  </Suspense>
);
