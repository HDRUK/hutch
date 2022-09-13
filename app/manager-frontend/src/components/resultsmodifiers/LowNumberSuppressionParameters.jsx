import { FormikInput } from "components/forms/FormikInput";


export const LowNumberSuppressionParameters = () => {
  return (
    <FormikInput
      label="Threshold"
      name={"Parameters.Threshold"}
      type="number"
    />
  );
};
