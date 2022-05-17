import { Button } from "@chakra-ui/react";
import { useLocation, useResolvedPath, Link } from "react-router-dom";

export const NavButton = ({ to, end, caseSensitive, ...p }) => {
  // essentially we have to roll our own equivalent of React Router's NavLink
  // using their utility hooks, because the NavLink component (which does the same)
  // only allows using its active state to set CSS via classes or direct styles
  // whereas we want to tell our component library/theming system the active state
  // :|

  const location = useLocation();
  const path = useResolvedPath(to);

  let locationPathname = location.pathname;
  let toPathname = path.pathname;
  if (!caseSensitive) {
    locationPathname = locationPathname.toLowerCase();
    toPathname = toPathname.toLowerCase();
  }

  const isActive =
    locationPathname === toPathname ||
    (!end &&
      locationPathname.startsWith(toPathname) &&
      locationPathname.charAt(toPathname.length) === "/");

  return <Button as={Link} to={to} {...p} isActive={isActive} />;
};
