import { FormikInput } from "components/forms/FormikInput";

export const RoundingParameters = () => {
  return (
    <FormikInput
      label="Rounding Parameter"
      name={"Parameters.nearest"}
      type="number"
    />
  );
};
