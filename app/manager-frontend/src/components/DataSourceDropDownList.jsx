import { useState } from "react";
import { useCombobox } from "downshift";
import {
  Box,
  Flex,
  Input,
  InputGroup,
  InputRightElement,
  List,
  ListItem,
  Stack,
  Text,
  VStack,
  Tooltip,
} from "@chakra-ui/react";
import { FaCircle } from "react-icons/fa";
import { useField } from "formik";
import { getTimeAgo } from "helpers/dates";

const ChevronDown = (p) => (
  <svg fill="none" viewBox="0 0 24 24" height="1em" width="1em" {...p}>
    <path
      fill="currentColor"
      d="M6.343 7.757L4.93 9.172 12 16.242l7.071-7.07-1.414-1.415L12 13.414 6.343 7.757z"
    />
  </svg>
);

export const DataSourceDropDownList = ({
  onSelection,
  label,
  options,
  width = "100%",
  name,
}) => {
  const [inputItems, setInputItems] = useState(options);
  const {
    isOpen,
    getMenuProps,
    getInputProps,
    getComboboxProps,
    highlightedIndex,
    getItemProps,
    openMenu,
  } = useCombobox({
    items: inputItems,
    itemToString: (option) => option.label,
    onSelectedItemChange: ({ selectedItem }) => {
      onSelection(JSON.stringify(selectedItem));
    },
    onInputValueChange: ({ inputValue }) => {
      setInputItems(
        options.filter((option) =>
          option.label.toLowerCase().startsWith(inputValue.toLowerCase())
        )
      );
    },
  });
  const [field,meta,helpers] = useField(name, { type: "select" });
  const [value, setValue] = useState(field.value);
  const handleChange = ({ target: { value } }) => {
    console.log(field)
    setValue(value);
    
    helpers.setValue(value);
  };
  return (
    <Stack w={width} {...getComboboxProps()} zIndex={10}>
      <InputGroup>
        <Input
          cursor="default"
          {...getInputProps({
            onClick: () => {
              if (!isOpen) {
                openMenu();
                setInputItems(options);
              }
            },
          })}
          placeholder={label}
          onSelect={handleChange}
        />
        <InputRightElement pointerEvents="none">
          <Flex boxSize="24px" align="center">
            <ChevronDown />
          </Flex>
        </InputRightElement>
      </InputGroup>

      <Box position="relative">
        <List
          {...getMenuProps()}
          position="absolute"
          width="100%"
          shadow="md"
          backgroundColor="white"
          borderColor="gray.200"
          borderWidth={1}
          borderRadius={"10px"}
          zIndex="portal"
          py={2}
          maxHeight="300px"
          overflowY="auto"
          hidden={!isOpen}
        >
          {inputItems.map((item, index) => (
            <ListItem
              key={item.value}
              p={2}
              bg={highlightedIndex === index ? "gray.100" : undefined}
              cursor="default"
              {...getItemProps({ item, index })}
              borderRadius={"10px"}
            >
              <VStack alignItems={"flex-start"}>
                <Flex alignItems={"baseline"}>
                  {item.status ? (
                    <Flex>
                      <Tooltip label={"Active"} placement={"top"}>
                        <span>
                          <FaCircle fontSize={"13px"} color="green" />
                        </span>
                      </Tooltip>
                    </Flex>
                  ) : (
                    <Flex>
                      <Tooltip label={"Inactive"} placement={"top"}>
                        <span>
                          <FaCircle fontSize={"13px"} color="lightslategray" />
                        </span>
                      </Tooltip>
                    </Flex>
                  )}
                  <Text fontSize="16px" pl={2}>
                    {item.label}
                  </Text>
                </Flex>
                <Flex alignItems={"baseline"}>
                  <Text
                    textTransform={"capitalize"}
                    fontWeight={500}
                    fontSize="13px"
                  >
                    Last Check-in:
                  </Text>
                  <Text pl={1} fontSize="12px">
                    {getTimeAgo(item.lastCheckIn)}
                  </Text>
                </Flex>
              </VStack>
            </ListItem>
          ))}
        </List>
      </Box>
    </Stack>
  );
};
