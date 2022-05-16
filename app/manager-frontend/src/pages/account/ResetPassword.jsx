import {
  Alert,
  AlertIcon,
  Button,
  Container,
  Heading,
  Text,
  VStack,
} from "@chakra-ui/react";
import { useLocation, useNavigate } from "react-router-dom";
import { BiReset } from "react-icons/bi";
import { Form, Formik } from "formik";
import { useTranslation } from "react-i18next";
import { object } from "yup";
import { useResetState } from "helpers/hooks/useResetState";
import { useUser } from "contexts/User";
import { ResendConfirmAlert } from "components/account/ResendConfirmAlert";
import {
  PasswordFieldGroup,
  validationSchema as pwSchema,
} from "components/forms/PasswordFieldGroup";
import { useQueryStringViewModel } from "helpers/hooks/useQueryStringViewModel";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";

const validationSchema = (t) => object().shape(pwSchema(t));

const InvalidLinkFeedback = () => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <TitledAlert status="error" title={t("feedback.error_title")}>
        <Text>{t("resetPassword.feedback.invalidLink")}</Text>
      </TitledAlert>
    </Container>
  );
};

const SimpleAlert = ({ status, message }) => (
  <Alert status={status}>
    <AlertIcon />
    {message}
  </Alert>
);

const FeedbackAlerts = ({ feedback, userId }) => (
  <>
    {feedback?.alerts &&
      feedback.alerts.map((alert, i) => (
        <SimpleAlert key={`alert_${i}`} {...alert} />
      ))}
    {feedback?.status && <SimpleAlert {...feedback} />}
    {feedback?.resendConfirm && <ResendConfirmAlert userIdOrEmail={userId} />}
  </>
);

export const ResetPassword = () => {
  const { userId, token } = useQueryStringViewModel();
  const { t } = useTranslation();
  const { key } = useLocation();
  const navigate = useNavigate();
  const { signIn } = useUser();
  const {
    account: { resetPassword },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  if (!userId || !token) return <InvalidLinkFeedback />;

  const handleSubmit = async ({ password, passwordConfirm }, actions) => {
    // If submission was triggered by hitting Enter,
    // we should blur the focused input
    // so we don't mess up validation when we reset after submission
    if (document?.activeElement) document.activeElement.blur();

    try {
      const { user, isUnconfirmedAccount } = await resetPassword(
        userId,
        token,
        password,
        passwordConfirm
      ).json();

      if (isUnconfirmedAccount) {
        setFeedback({
          alerts: [
            {
              status: "success",
              message: t("resetPassword.feedback.success"),
            },
            {
              status: "warning",
              message: t("feedback.account.unconfirmed"),
            },
          ],

          resendConfirm: true,
          hideForm: true,
        });
      } else {
        // no gotchas? Sign In and be on our way
        signIn(user);
        navigate("/", {
          state: {
            toast: {
              title: `${t("resetPassword.feedback.success")} ${t(
                "resetPassword.feedback.loggedIn"
              )}`,
              status: "success",
              duration: 2500,
              isClosable: true,
            },
          },
        });
        return;
      }
    } catch (e) {
      console.error(e.response);
      switch (e.response?.status) {
        case 404:
        case 400:
          setFeedback({
            status: "error",
            message: t("resetPassword.feedback.invalidLink"),
          });
          break;
        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }

      window.scrollTo(0, 0);
    }

    // when we reset, untouch fields so that clicking away from an input
    // that we are emptying doesn't (in)validate that now empty input
    actions.resetForm({ touched: [] });

    actions.setSubmitting(false);
  };

  return (
    <Container key={key} my={8}>
      <VStack align="stretch" spacing={8}>
        <Heading as="h2" size="lg">
          {t("resetPassword.heading")}
        </Heading>

        <FeedbackAlerts feedback={feedback} userId={userId} />

        <Formik
          initialValues={{
            password: "",
            passwordConfirm: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={8} hidden={feedback?.hideForm}>
                <PasswordFieldGroup
                  initialHidden={feedback?.status !== "error"}
                />

                <Button
                  w="200px"
                  colorScheme="blue"
                  leftIcon={<BiReset />}
                  type="submit"
                  disabled={isSubmitting}
                  isLoading={isSubmitting}
                >
                  {t("resetPassword.buttons.submit")}
                </Button>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
