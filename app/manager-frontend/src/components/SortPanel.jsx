import { Button, ButtonGroup } from "@chakra-ui/react";
import { FaCaretUp, FaCaretDown } from "react-icons/fa";

const SortButton = ({ active, asc, children, onClick, ...rest }) => (
  <Button
    rightIcon={asc ? <FaCaretUp /> : <FaCaretDown />}
    _focus={{}}
    lineHeight="inherit"
    borderRadius={0}
    fontWeight={active ? "bold" : "normal"}
    onClick={() => onClick(active ? !asc : asc)}
    isActive={active}
    {...rest}
  >
    {children}
  </Button>
);

export const SortPanel = ({ keys = [], state, onSortButtonClick }) => {
  const createSortButton = (label, key) => {
    return (
      <SortButton
        key={key}
        active={state.key === key}
        asc={state[key]}
        onClick={() => onSortButtonClick(key)}
      >
        {label}
      </SortButton>
    );
  };

  return (
    <ButtonGroup size="sm" isAttached>
      {keys.map((sortKey) => {
        return typeof sortKey === "string"
          ? createSortButton(sortKey, sortKey.toLocaleLowerCase())
          : createSortButton(sortKey[0], sortKey[1]);
      })}
    </ButtonGroup>
  );
};
