import React from "react"; // eslint-disable-line no-unused-vars
import { useHistory } from "@docusaurus/router";
import { Button } from "@chakra-ui/react";

/** A Chakra Button for a Docusaurus internal (React Router) Link */
const LinkButton = ({ href = "", to = "", ...p }) => {
  const history = useHistory();
  const handleClick = () => {
    if (to) history.push(to);
    else location.href = href;
  };

  return <Button cursor="pointer" onClick={handleClick} {...p} />;
};

export default LinkButton;
