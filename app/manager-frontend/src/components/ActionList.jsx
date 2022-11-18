import {
  Heading,
  Button,
  Input,
  HStack,
  Text,
  InputGroup,
  InputLeftElement,
  Stack,
  Box,
} from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { FaPlus, FaSearch, FaInfoCircle } from "react-icons/fa";

export const ActionList = ({
  data,
  setFilter,
  href,
  actionTitle,
  actionNewTitle,
  modalDelete: ModalDelete,
  children,
}) => {
  const navigate = useNavigate();

  return (
    <>
      {data ? (
        <Stack w="100%" spacing={4}>
          <HStack maxW="800" w="100%" alignSelf="center" borderRadius="10px">
            <InputGroup>
              <InputLeftElement pointerEvents="none">
                <FaSearch color="gray.300" />
              </InputLeftElement>
              <Input
                size="md"
                placeholder={`Search ${actionTitle}`}
                onChange={(e) => setFilter(e.target.value)}
              />
            </InputGroup>

            <Button
              onClick={() => navigate(`${href}/new`)}
              colorScheme="green"
              leftIcon={<FaPlus />}
            >
              <Text
                textTransform="uppercase"
                fontWeight={700}
                fontSize="sm"
                letterSpacing={1.1}
              >
                New
              </Text>
            </Button>
          </HStack>
          {children}
          <ModalDelete />
        </Stack>
      ) : (
        <Box textAlign="center" py={10} px={6}>
          <FaInfoCircle
            fontSize="2em"
            color="dodgerblue"
            style={{ display: "inline" }}
          />
          <Heading as="h2" size="xl" mb={2}>
            No {actionTitle} found.
          </Heading>
          <Button
            onClick={() => navigate(`${href}/new`)}
            colorScheme="green"
            leftIcon={<FaPlus />}
            width="225"
          >
            <Text
              textTransform="uppercase"
              fontWeight={700}
              fontSize="sm"
              letterSpacing={1.1}
            >
              {actionNewTitle}
            </Text>
          </Button>
        </Box>
      )}
    </>
  );
};
