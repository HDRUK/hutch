import { FormControl, FormLabel, Input, Flex, Tooltip } from "@chakra-ui/react";
import { useField } from "formik";
import { useDebounce } from "helpers/hooks/useDebounce";
import { useEffect, useState } from "react";
import { FormHelpError } from "./FormHelpError";
import { Icon } from "@chakra-ui/react";
import { AiFillInfoCircle } from "react-icons/ai";

export const FormikInput = ({
  name,
  label,
  placeholder,
  type = "text",
  isRequired,
  fieldTip,
  fieldHelp,
  collapseError,
  tooltip,
  ...p
}) => {
  const [field, meta, helpers] = useField({ name, type });

  const [value, setValue] = useState(field.value);
  const debouncedValue = useDebounce(value, 150);

  const handleChange = ({ target: { value } }) => {
    setValue(value);
  };

  useEffect(() => {
    helpers.setValue(debouncedValue);
  }, [debouncedValue]);

  return (
    <FormControl
      id={field.name}
      isRequired={isRequired}
      isInvalid={meta.error && meta.touched}
    >
      <Flex w={"full"}>
        {label && <FormLabel>{label}</FormLabel>}

        {tooltip && (
          <Flex ml={"auto"} alignItems="center">
            <Tooltip label={tooltip}>
              <Icon as={AiFillInfoCircle} />
            </Tooltip>
          </Flex>
        )}
      </Flex>
      <Input
        type={type}
        placeholder={placeholder}
        {...p}
        value={value}
        onChange={handleChange}
      />
      {fieldTip}

      <FormHelpError
        isInvalid={meta.error && meta.touched}
        error={meta.error}
        help={fieldHelp}
        collapseEmpty={collapseError}
        replaceHelpWithError
      />
    </FormControl>
  );
};
