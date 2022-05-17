import { Container, Text, VStack } from "@chakra-ui/react";
import { ResendConfirmAlert } from "components/account/ResendConfirmAlert";
import { BusyPage } from "components/Busy";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";
import { useUser } from "contexts/User";
import { useQueryStringViewModel } from "helpers/hooks/useQueryStringViewModel";
import { Suspense, useCallback } from "react";
import { useAsync } from "react-async";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

const ErrorFeedback = ({ tKey, userId }) => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <VStack spacing={4}>
        <TitledAlert status="error" title={t("feedback.error_title")}>
          <Text>{t(tKey ?? "confirm.feedback.error")}</Text>
        </TitledAlert>

        {userId && <ResendConfirmAlert userIdOrEmail={userId} />}
      </VStack>
    </Container>
  );
};

const useConfirmHandler = () => {
  const {
    account: { confirm },
  } = useBackendApi();
  return useCallback(
    async ({ userId, token }) => await confirm(userId, token).json(),
    [confirm]
  );
};

// this actually does the hard work
// but we use suspense at the page level while its busy
const ConfirmAccount = () => {
  const { userId, token } = useQueryStringViewModel();
  const { signIn } = useUser();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleConfirm = useConfirmHandler();

  const { error, data } = useAsync(handleConfirm, {
    suspense: true,
    userId,
    token,
  });

  if (error) {
    let tKey;
    switch (error?.response?.status) {
      case 400:
      case 404:
        tKey = "confirm.feedback.invalidLink";
        break;
    }

    console.error(error);
    return <ErrorFeedback tKey={tKey} userId={userId} />;
  }

  if (data) {
    // log in, redirect home and TOAST success
    signIn(data);
    navigate("/", {
      state: {
        toast: {
          title: t("confirm.feedback.success"),
          status: "success",
          duration: 2500,
          isClosable: true,
        },
      },
    });
  }

  return null;
};

export const Confirm = () => (
  <Suspense
    fallback={
      <BusyPage
        tKey="confirm.feedback.busy"
        containerProps={{ justifyContent: "center" }}
      />
    }
  >
    <ConfirmAccount />
  </Suspense>
);
