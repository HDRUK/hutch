import {
  Heading,
  HStack,
  Text,
  LinkBox,
  LinkOverlay,
  VStack,
  Box,
  Button,
  Flex,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";

export const ActivitySourceSummary = ({
  title,
  href,
  sourceURL,
  collectionId,
  children,
  onDelete,
  ...p
}) => (
  <LinkBox>
    <VStack
      bg="gray.100"
      borderColor="gray.300"
      borderWidth={2}
      borderRadius={5}
      h="100%"
      p={4}
      align="stretch"
      _hover={{
        borderColor: "blue.500",
        color: "blue.500",
        bg: "gray.50",
      }}
      {...p}
    >
      <HStack>
        <Flex w="full">
          <Heading as="h3" size="md">
            <LinkOverlay w="100%" as={Link} to={`${href}`}>
              {title}
            </LinkOverlay>
          </Heading>
          <Button colorScheme="red" ml={"auto"} onClick={onDelete}>
            Delete
          </Button>
        </Flex>
      </HStack>
      {children}
      {sourceURL && <Text>Source: {sourceURL}</Text>}
      {collectionId && <Text>collection id: {collectionId}</Text>}
    </VStack>
  </LinkBox>
);
