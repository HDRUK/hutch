import { object, string } from "yup";

export const validationSchema = () =>
  object().shape({
    Name: string().required("Please provide an Agent name"),
  });
