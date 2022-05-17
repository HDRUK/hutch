import {
  Alert,
  AlertIcon,
  Button,
  Container,
  Heading,
  VStack,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { useLocation } from "react-router-dom";
import { BiMailSend } from "react-icons/bi";
import { useTranslation } from "react-i18next";
import { object, string } from "yup";
import { useResetState } from "helpers/hooks/useResetState";
import { EmailField } from "components/forms/EmailFieldGroup";
import { useBackendApi } from "contexts/BackendApi";

const validationSchema = (t) =>
  object().shape({
    email: string()
      .email(t("validation.email_valid"))
      .required(t("validation.email_required")),
  });

export const RequestPasswordReset = () => {
  const { key } = useLocation();
  const { t } = useTranslation();
  const {
    account: { requestPasswordReset },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  const handleSubmit = async ({ email }, actions) => {
    // If submission was triggered by hitting Enter,
    // we should blur the focused input
    // so we don't mess up validation when we reset after submission
    if (document?.activeElement) document.activeElement.blur();

    let success = false;

    try {
      await requestPasswordReset(email);

      success = true;
    } catch (e) {
      switch (e?.response?.status) {
        case 404:
          // We didn't find the user, but we don't want to expose that
          success = true;
          break;
        case 400:
          setFeedback({
            status: "error",
            message: t("feedback.invalidForm"),
          });
          break;

        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }
    }

    if (success) {
      setFeedback({
        status: "success",
        message: t("requestPasswordReset.feedback.success"),
      });

      // when we reset, untouch fields so that clicking away from an input
      // that we are emptying doesn't (in)validate that now empty input
      actions.resetForm({ touched: [] });
    }

    window.scrollTo(0, 0);

    actions.setSubmitting(false);
  };

  return (
    <Container key={key} my={8}>
      <VStack align="stretch" spacing={8}>
        <Heading as="h2" size="lg">
          {t("requestPasswordReset.heading")}
        </Heading>

        {feedback?.status && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}

        <Formik
          initialValues={{
            email: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={8}>
                <EmailField
                  label={t("fields.email")}
                  placeholder={t("fields.email_placeholder")}
                />

                <Button
                  w="200px"
                  colorScheme="blue"
                  leftIcon={<BiMailSend />}
                  type="submit"
                  disabled={isSubmitting}
                  isLoading={isSubmitting}
                >
                  {t("requestPasswordReset.buttons.submit")}
                </Button>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
