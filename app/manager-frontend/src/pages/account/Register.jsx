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
import { Form, Formik } from "formik";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { FaUserPlus, FaSignInAlt } from "react-icons/fa";
import { useTranslation } from "react-i18next";
import { object, string } from "yup";
import { useResetState } from "helpers/hooks/useResetState";
import { FormikInput } from "components/forms/FormikInput";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";
import {
  EmailField,
  validationSchema as emailSchema,
} from "components/forms/EmailField";
import {
  PasswordField,
  validationSchema as pwSchema,
} from "components/forms/PasswordField";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";
import { HutchLogo } from "components/Logo";

export const validationSchema = (t) =>
  object().shape({
    fullname: string().required(t("validation.fullname_required")),
    ...emailSchema(t),
    ...pwSchema(t),
  });

export const Register = () => {
  const { key } = useLocation();
  const { t } = useTranslation();
  const {
    account: { register },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const handleSubmit = async (values, actions) => {
    // If submission was triggered by hitting Enter,
    // we should blur the focused input
    // so we don't mess up validation when we reset after submission
    if (document?.activeElement) document.activeElement.blur();

    try {
      const response = await register(values);

      setFeedback({
        status: "success",
        message: t("register.feedback.success"),
        confirmationRequired: response.status == 202, // True if email confirmation required else False
      });

      // when we reset, untouch fields so that clicking away from an input
      // that we are emptying doesn't (in)validate that now empty input
      actions.resetForm({ touched: [] });
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          const result = await e.response.json();

          let message = t("feedback.invalidForm");
          if (result.isExistingUser)
            message = t("register.feedback.existingUser");
          else if (result.isNotAllowlisted)
            message = t("register.feedback.emailNotAllowlisted");

          setFeedback({ status: "error", message });

          break;
        }

        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }
    }

    scrollTargetIntoView();

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
          {t("register.heading")}
        </Heading>

        {feedback?.status && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}
        {feedback?.confirmationRequired && (
          <TitledAlert title={t("register.feedback.confirm_title")}>
            {t("register.feedback.confirm_message")}
          </TitledAlert>
        )}

        <Formik
          initialValues={{
            fullname: "",
            email: "",
            password: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={4}>
                <EmailField hasCheckReminder autoFocus />

                <FormikInput
                  name="fullname"
                  label={t("register.fields.fullname")}
                  placeholder={t("register.fields.fullname_placeholder")}
                  isRequired
                />

                <PasswordField />

                <HStack justify="space-between">
                  <Button
                    w="150px"
                    colorScheme="blue"
                    leftIcon={<FaUserPlus />}
                    type="submit"
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {t("buttons.register")}
                  </Button>

                  <Button
                    colorScheme="blue"
                    variant="link"
                    leftIcon={<FaSignInAlt />}
                  >
                    <Link as={RouterLink} to="/account/login">
                      {t("register.links.login")}
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
