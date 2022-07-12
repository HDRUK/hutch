import { countWords } from "helpers/strings";
import { array, number, object, string } from "yup";

export const validationSchema = () =>
  object().shape({
    Parameters: object().shape({
      Threshold: number().integer().moreThan(0),
    }),
  });
