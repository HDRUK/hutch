import { useEffect, useState } from "react";

/**
 * `useState` that resets to its initial value when any `deps` values change
 * @param {*} deps array of dependent values that will trigger a state reset on change
 * @param {*} init initial state value
 * @returns `[state, setState]`
 */
export const useResetState = (deps, init) => {
  const [state, setState] = useState(init);
  useEffect(() => {
    setState(init);
  }, deps);

  return [state, setState];
};
