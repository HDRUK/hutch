import {
  Alert,
  AlertIcon,
  Button,
  Center,
  Container,
  Heading,
  HStack,
  Link,
  VStack,
} from "@chakra-ui/react";
import { Link as RouterLink, useLocation, useNavigate } from "react-router-dom";
import { FaSignInAlt, FaUserPlus } from "react-icons/fa";
import { PasswordField } from "components/forms/PasswordField";
import { Form, Formik } from "formik";
import { useTranslation } from "react-i18next";
import { object, string } from "yup";
import { useResetState } from "helpers/hooks/useResetState";
import { useUser } from "contexts/User";
import { ResendConfirmAlert } from "components/account/ResendConfirmAlert";
import { useBackendApi } from "contexts/BackendApi";
import { EmailField } from "components/forms/EmailField";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { HutchLogo } from "components/Logo";

const validationSchema = (t) =>
  object().shape({
    username: string()
      .test(
        "valid-username",
        t("validation.email_valid"),
        (v) =>
          // this allows for DECSYS style "@admin" usernames in future
          // for a non-email seeded superuser
          string().email().isValidSync(v) ||
          string().matches(/^@/).isValidSync(v)
      )
      .required(t("validation.email_required")),
    password: string().required(t("validation.password_required")),
  });

export const Login = () => {
  const { t } = useTranslation();
  const { key, state } = useLocation();
  const navigate = useNavigate();
  const { signIn } = useUser();
  const {
    account: { login },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const handleSubmit = async (values, actions) => {
    try {
      const { user } = await login(values).json();
      signIn(user);

      // redirect back to where we came from; otherwise the user home
      navigate(state?.from ?? "/");
      return;
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          const result = await e.response.json();

          setFeedback({
            status: "error",
            message: result.isUnconfirmedAccount
              ? t("feedback.account.unconfirmed")
              : t("login.feedback.loginFailed"),
            resendConfirm: result.isUnconfirmedAccount,
          });

          break;
        }
        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }

      scrollTargetIntoView();
    }

    actions.setSubmitting(false);
  };

  return (
    <Container maxWidth="md" ref={scrollTarget} key={key} my={8}>
      <VStack align="stretch" spacing={4}>
        <Center>
          <HutchLogo
            logoColor={true}
            logoMaxWidth="170px"
            logoFillColor="#000"
          />
        </Center>

        <Heading as="h2" size="lg">
          {t("login.heading")}
        </Heading>

        <Formik
          initialValues={{
            username: "",
            password: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting, values }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                {feedback?.status && (
                  <Alert status={feedback.status}>
                    <AlertIcon />
                    {feedback.message}
                  </Alert>
                )}
                {feedback?.resendConfirm && (
                  <ResendConfirmAlert userIdOrEmail={values.username} />
                )}

                <EmailField name="username" autoFocus />

                <PasswordField
                  fieldTip={
                    <Link as={RouterLink} to="/account/password/reset">
                      {t("login.links.forgotPassword")}
                    </Link>
                  }
                />

                <HStack justify="space-between">
                  <Button
                    w="200px"
                    colorScheme="blue"
                    leftIcon={<FaSignInAlt />}
                    type="submit"
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {t("buttons.login")}
                  </Button>
                  <Button
                    colorScheme="blue"
                    variant="link"
                    leftIcon={<FaUserPlus />}
                  >
                    <Link as={RouterLink} to="/account/register">
                      {t("login.links.register")}
                    </Link>
                  </Button>
                </HStack>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
