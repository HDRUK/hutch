/**
 * Promise based wrapper for the FileReader API
 * @param {*} file
 * @returns
 */
export const readFileAsTextAsync = (file) =>
  new Promise((resolve, reject) => {
    let reader = new FileReader();

    reader.onload = () => {
      resolve(reader.result);
    };

    reader.onerror = reject;

    reader.readAsText(file);
  });
