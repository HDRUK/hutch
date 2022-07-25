import { HStack, Icon, Text } from "@chakra-ui/react";
import { useTranslation } from "react-i18next";
import { FaExclamationTriangle } from "react-icons/fa";
import { string } from "yup";
import { FormikInput } from "./FormikInput";

export const validationSchema = (t) => ({
  email: string()
    .email(t("validation.email_valid"))
    .required(t("validation.email_required")),
});

export const EmailField = ({ name = "email", hasCheckReminder, ...p }) => {
  const { t } = useTranslation();

  const checkReminder = (
    <HStack>
      <Icon as={FaExclamationTriangle} />
      <Text>{t("fields.email_checkreminder")}</Text>
    </HStack>
  );

  return (
    <FormikInput
      name={name}
      type="email"
      isRequired
      label={t("fields.email")}
      placeholder={t("fields.email_placeholder")}
      fieldHelp={hasCheckReminder ? checkReminder : undefined}
      {...p}
    />
  );
};
