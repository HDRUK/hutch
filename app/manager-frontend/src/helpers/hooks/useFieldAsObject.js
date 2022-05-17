import { useField } from "formik";

/**
 * Formik's `useField` hook but it returns a dictionary instead of an array
 * @param {*} options
 * @returns
 */
export const useFieldAsObject = (options) => {
  const array = useField(options);
  return {
    field: array[0],
    meta: array[1],
    helpers: array[2],
  };
};
