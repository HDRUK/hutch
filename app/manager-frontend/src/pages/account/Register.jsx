import {
  Alert,
  AlertIcon,
  Button,
  Container,
  Heading,
  HStack,
  Link,
  VStack,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { FaUserPlus } from "react-icons/fa";
import { useTranslation } from "react-i18next";
import { object, string, ref } from "yup";
import { useResetState } from "helpers/hooks/useResetState";
import { FormikInput } from "components/forms/FormikInput";
import { EmailFieldGroup } from "components/forms/EmailFieldGroup";
import {
  PasswordFieldGroup,
  validationSchema as pwSchema,
} from "components/forms/PasswordFieldGroup";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";

export const validationSchema = (t) =>
  object().shape({
    fullname: string().required(t("validation.fullname_required")),
    email: string()
      .email(t("validation.email_valid"))
      .required(t("validation.email_required")),
    emailConfirm: string()
      .oneOf([ref("email")], t("validation.emailconfirm_match"))
      .required(t("validation.emailconfirm_required")),
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

  const handleSubmit = async (values, actions) => {
    // If submission was triggered by hitting Enter,
    // we should blur the focused input
    // so we don't mess up validation when we reset after submission
    if (document?.activeElement) document.activeElement.blur();

    try {
      await register(values);

      setFeedback({
        status: "success",
        message: t("register.feedback.success"),
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

    window.scrollTo(0, 0);

    actions.setSubmitting(false);
  };

  return (
    <Container key={key} my={8}>
      <VStack align="stretch" spacing={8}>
        <Heading as="h2" size="lg">
          {t("register.heading")}
        </Heading>

        {feedback?.status && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}
        {feedback?.status === "success" && (
          <TitledAlert title={t("register.feedback.confirm_title")}>
            {t("register.feedback.confirm_message")}
          </TitledAlert>
        )}

        <Formik
          initialValues={{
            fullname: "",
            email: "",
            emailConfirm: "",
            password: "",
            passwordConfirm: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={8}>
                <FormikInput
                  name="fullname"
                  label={t("register.fields.fullname")}
                  placeholder={t("register.fields.fullname_placeholder")}
                  isRequired
                />

                <EmailFieldGroup initialHidden={feedback?.status !== "error"} />

                <PasswordFieldGroup
                  initialHidden={feedback?.status !== "error"}
                />

                <HStack justify="space-between">
                  <Button
                    w="200px"
                    colorScheme="blue"
                    leftIcon={<FaUserPlus />}
                    type="submit"
                    disabled={isSubmitting}
                    isLoading={isSubmitting}
                  >
                    {t("buttons.register")}
                  </Button>

                  <Link as={RouterLink} to="/account/login">
                    {t("register.links.login")}
                  </Link>
                </HStack>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
