import { HStack, Icon, Text } from "@chakra-ui/react";
import { useTranslation } from "react-i18next";
import { FaExclamationTriangle } from "react-icons/fa";
import { string } from "yup";
import { FormikInput } from "./FormikInput";

export const validationSchema = (t) => ({
  username: string()
    .username(t("validation.username"))
    .required(t("validation.username_required")),
});

export const UsernameField = ({
  name = "username",
  hasCheckReminder,
  ...p
}) => {
  const { t } = useTranslation();

  const checkReminder = (
    <HStack>
      <Icon as={FaExclamationTriangle} />
      <Text>{t("fields.username_checkreminder")}</Text>
    </HStack>
  );

  return (
    <FormikInput
      name={name}
      type="username"
      isRequired
      label={t("fields.username")}
      placeholder={t("fields.username_placeholder")}
      fieldHelp={hasCheckReminder ? checkReminder : undefined}
      {...p}
    />
  );
};
