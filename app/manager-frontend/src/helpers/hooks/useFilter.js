import { useEffect, useState } from "react";

/**
 * Filters a list of objects with a `key` property
 * where the `key` property contains a match on the `filter` string.
 *
 * If no filter is specified, the full input list is returned.
 *
 * @param {object[]} input The source list of objects.
 * @param {string} filter The filter string to match against.
 * @param {string} key The key property name
 */
export const getFilteredLookup = (input, filter, key) => {
  const filtered = input.filter((item) =>
    new RegExp(filter, "i").test(item[key])
  );

  return !filter ? input : filtered;
};

// TODO: add sorting state too
export const useFilter = (data) => {
  const [filter, setFilter] = useState("");
  const [filteredData, setFilteredData] = useState([]);

  useEffect(() => {
    setFilteredData(
      getFilteredLookup(
        Object.keys(data).map((id) => data[id]),
        filter,
        "name"
      )
    );
  }, [filter, data]);

  return {
    filter,
    setFilter,
    filteredData,
  };
};
