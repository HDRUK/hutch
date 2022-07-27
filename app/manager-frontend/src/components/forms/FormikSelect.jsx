import {
  FormControl,
  FormLabel,
  Flex,
  Select,
  Tooltip,
  Icon,
  Alert,
  AlertIcon,
  VStack,
} from "@chakra-ui/react";
import { useField } from "formik";
import { useState } from "react";
import { FormHelpError } from "./FormHelpError";
import { FaInfoCircle } from "react-icons/fa";

export const FormikSelect = ({
  name,
  label,
  options,
  isRequired,
  fieldTip,
  fieldHelp,
  collapseError,
  tooltip,
  alert,
  hasEmptyDefault,
}) => {
  const [field, meta, helpers] = useField({ name, type: "select" });
  const [value, setValue] = useState(field.value);

  const handleChange = ({ target: { value } }) => {
    setValue(value);
    helpers.setValue(value);
  };

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
              <span>
                <Icon as={FaInfoCircle} />
              </span>
            </Tooltip>
          </Flex>
        )}
      </Flex>

      <VStack>
        {alert?.message && (
          <Alert status={alert.status ?? "info"} variant="left-accent">
            <AlertIcon />
            {alert.message}
          </Alert>
        )}

        <Select value={value} onChange={handleChange}>
          {hasEmptyDefault && <option value=""></option>}
          {options.map((item, index) => (
            <option key={index} value={item.value}>
              {item.label}
            </option>
          ))}
        </Select>
      </VStack>
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
