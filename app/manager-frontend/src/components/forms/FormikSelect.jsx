import { FormControl, FormLabel, Input, Select } from "@chakra-ui/react";
import { useField } from "formik";
import { useDebounce } from "helpers/hooks/useDebounce";
import { useEffect, useState } from "react";
import { FormHelpError } from "./FormHelpError";

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
      {label && <FormLabel>{label}</FormLabel>}
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
