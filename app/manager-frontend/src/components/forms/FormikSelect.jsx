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
  sourceList,
  sourceKey,
}) => {
  const [field, meta, helpers] = useField({ name, type: "select" });
  const initialValue =
    sourceList && sourceKey && typeof field.value === "object"
      ? sourceList.find((item) => item[sourceKey] == field.value[sourceKey])[
          sourceKey
        ]
      : field.value;
  const [value, setValue] = useState(initialValue);

  const handleChange = ({ target: { value } }) => {
    let formikValue = value;
    if (sourceList && sourceKey) {
      formikValue = sourceList.find((item) => item[sourceKey] == value);
    }
    setValue(value);
    helpers.setValue(formikValue);
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
