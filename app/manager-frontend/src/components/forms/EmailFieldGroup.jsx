import { Flex } from "@chakra-ui/react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { FormikInput } from "./FormikInput";

export const EmailField = ({ name = "email", ...p }) => (
  <FormikInput name={name} type="email" isRequired {...p} />
);

export const EmailFieldGroup = ({ initialHidden }) => {
  const { t } = useTranslation();

  const [hidden, setHidden] = useState(initialHidden);
  const handleFocus = () => setHidden(false);

  return (
    <>
      <EmailField
        label={t("fields.email")}
        placeholder={t("fields.email_placeholder")}
        onFocus={handleFocus}
      />

      <Flex hidden={hidden}>
        <EmailField
          name="emailConfirm"
          label={t("fields.email_confirm")}
          placeholder={t("fields.email_placeholder")}
        />
      </Flex>
    </>
  );
};
