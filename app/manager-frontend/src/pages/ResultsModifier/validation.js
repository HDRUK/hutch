import { array, boolean, number, object, string } from "yup";

export const validationSchema = () =>
  object().shape({
    Type: string(),
    Parameters: object().shape({
      threshold: object().when("Type", (type) => {
        if (type === "Low Number Suppression") {
          return number().integer().moreThan(0).required();
        } else {
          return string().nullable();
        }
      }),
      nearest: object().when("Type", (type) => {
        if (type === "Rounding") {
          return number().integer().moreThan(0).required();
        } else {
          return string().nullable();
        }
      }),
    }),
  });
