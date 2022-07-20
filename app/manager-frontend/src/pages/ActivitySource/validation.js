import { object, string } from "yup";

const regMatch =
  /^((http|https):\/\/)?(www.)?(?!.*(http|https|www.))([a-zA-Z0-9_-]+:[a-zA-Z0-9_-]+@)?[a-zA-Z0-9_-]+(\.[a-zA-Z]+)+(\/)?.([\w\?[a-zA-Z-_%\/@?]+)*([^\/\w\?[a-zA-Z0-9_-]+=\w+(&[a-zA-Z0-9_]+=\w+)*)?$/;
export const validationSchema = () =>
  object().shape({
    DisplayName: string().required("Please provide a display name."),
    Host: string()
      .matches(regMatch, "Invalid URL")
      .required("Please provide a source url."),
    ResourceId: string().required("Please provide a valid resource id."),
    TargetDataSource: string().required("Please select a Data Source"),
  });
