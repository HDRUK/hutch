import {
  FormControl,
  FormLabel,
  Flex,
  Select,
  Tooltip,
  Icon,
} from "@chakra-ui/react";
import { useField } from "formik";
import { useEffect, useState } from "react";
import { FormHelpError } from "./FormHelpError";
import { AiFillInfoCircle } from "react-icons/ai";

export const FormikSelect = ({
  name,
  label,
  options,
  placeholder,
  type = "select",
  isRequired,
  fieldTip,
  fieldHelp,
  collapseError,
  sourceList,
  sourceParam,
  tooltip,
  warning,
  ...p
}) => {
  const [field, meta, helpers] = useField({ name, type });
  const [value, setValue] = useState(field.value);

  const handleChange = ({ target: { value } }) => {
    let formikValue = value;
    if (sourceList && sourceParam) {
      formikValue = sourceList.find((item) => item[sourceParam] == value);
      setValue(value);
    } else {
      setValue(value);
    }
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
                <Icon as={AiFillInfoCircle} color={warning ? "red" : "black"} />
              </span>
            </Tooltip>
          </Flex>
        )}
      </Flex>
      <Select value={value} type={type} onChange={handleChange}>
        {options.map((item, index) => (
          <option key={index} value={item.value}>
            {item.label}
          </option>
        ))}
      </Select>
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
