import { useFormikContext } from "formik";
import { FormikInput } from "components/forms/FormikInput";


export const LowNumberSuppressionParameters = ({ type }) => {
  const formikProps = useFormikContext();
  switch (type) {
    case "Type1":
      return (
        <>
          <FormikInput
            label="Threshold"
            name={"Parameters.Threshold"}
            type="number"
          />
        </>
      );
    default:
      return (
        <>
          <FormikInput
            label="Threshold"
            name={"Parameters.Threshold"}
            type="number"
          />
        </>
      );
  }
};
