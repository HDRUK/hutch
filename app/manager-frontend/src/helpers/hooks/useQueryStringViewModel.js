import { useLocation } from "react-router-dom";
import queryString from "query-string";
import { Base64UrlToJson } from "helpers/data-structures";

export const useQueryString = () => queryString.parse(useLocation().search);

export const useQueryStringViewModel = (param = "vm") => {
  const value = useQueryString()[param];
  try {
    const decoded = Base64UrlToJson(value) ?? {};
    return decoded;
  } catch {
    console.error(`Failed to parse data from query parameter: ${param}`);
    return {};
  }
};
