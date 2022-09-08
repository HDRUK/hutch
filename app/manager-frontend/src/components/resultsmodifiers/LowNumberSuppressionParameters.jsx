import { useFormikContext } from "formik";
import { FormikInput } from "components/forms/FormikInput";


export const LowNumberSuppressionParameters = ({ type }) => {
  return (
    <>
      <FormikInput
        label="Threshold"
        name={"Parameters.Threshold"}
        type={type}
      />
    </>
  );


};
