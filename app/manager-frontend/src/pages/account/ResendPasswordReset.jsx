import { Container, Text } from "@chakra-ui/react";
import { BusyPage } from "components/Busy";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";
import { useQueryStringViewModel } from "helpers/hooks/useQueryStringViewModel";
import { Suspense, useCallback } from "react";
import { useAsync } from "react-async";
import { useTranslation } from "react-i18next";

const SuccessFeedback = () => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <TitledAlert
        status="success"
        title={t("resendPasswordReset.feedback.success_title")}
      >
        <Text>{t("resendPasswordReset.feedback.success_message")}</Text>
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
    account: { requestPasswordReset },
  } = useBackendApi();
  return useCallback(
    async ({ userId }) => {
      await requestPasswordReset(userId).json(); // returns 204
      return true; // any failure will throw before this point :)
    },
    [requestPasswordReset]
  );
};

// this actually does the hard work
// but we use suspense at the page level while its busy
const ResendPasswordResetLink = () => {
  const { userId } = useQueryStringViewModel();
  const handleResend = useResendHandler();

  const { error, data } = useAsync(handleResend, {
    suspense: true,
    userId,
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

export const ResendPasswordReset = () => (
  <Suspense
    fallback={
      <BusyPage
        tKey="resendPasswordReset.feedback.busy"
        containerProps={{ justifyContent: "center" }}
      />
    }
  >
    <ResendPasswordResetLink />
  </Suspense>
);
