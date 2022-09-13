import {
  Flex,
  FormControl,
  FormLabel,
  Icon,
  IconButton,
  Input,
  InputGroup,
  InputLeftElement,
  Tooltip,
} from "@chakra-ui/react";
import { useField } from "formik";
import { useDebounce } from "helpers/hooks/useDebounce";
import { useEffect, useState } from "react";
import { FaEye, FaEyeSlash, FaInfoCircle } from "react-icons/fa";
import { FormHelpError } from "./FormHelpError";

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

  const [isMasked, setIsMasked] = useState(type === "password");

  const [value, setValue] = useState(field.value);
  const debouncedValue = useDebounce(value, 150);

  const handleChange = ({ target: { value } }) => {
    setValue(value);
  };

  useEffect(() => {
    const outputValue =
      type === "number" ? parseFloat(debouncedValue) : debouncedValue;
    helpers.setValue(outputValue);
  }, [debouncedValue]);

  useEffect(() => {
    setValue(field.value);
  }, [field.value]);

  const inputField = (
    <Input
      type={isMasked ? "password" : type === "password" ? "text" : type}
      placeholder={placeholder}
      {...p}
      value={value}
      onChange={handleChange}
    />
  );

  return (
    <FormControl
      id={field.name}
      isRequired={isRequired}
      isInvalid={meta.error && meta.touched}
    >
      <Flex w="100%">
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
      {type === "password" ? (
        <InputGroup>
          {inputField}
          <InputLeftElement>
            <IconButton
              variant="solid"
              onClick={() => setIsMasked(!isMasked)}
              size="xs"
              icon={isMasked ? <FaEye /> : <FaEyeSlash />}
            />
          </InputLeftElement>
        </InputGroup>
      ) : (
        inputField
      )}

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
