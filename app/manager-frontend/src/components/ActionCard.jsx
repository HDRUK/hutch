import {
  Heading,
  HStack,
  Icon,
  LinkBox,
  LinkOverlay,
  VStack,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";

export const ActionCard = ({ icon, title, href, active, children, ...p }) => (
  <LinkBox>
    <VStack
      spacing={1}
      minW={250}
      bg={active && "gray.200"}
      color={active && "blue.900"}
      borderColor={!active && "gray.300"}
      borderWidth={1}
      borderRadius={5}
      h="100%"
      p={2.5}
      align="stretch"
      _hover={{
        borderColor: `${!active && "blue.500"}`,
        transition: "0.45s",
      }}
      {...p}
    >
      <HStack lineHeight="80%">
        {icon && <Icon as={icon} />}
        <LinkOverlay w="100%" as={Link} to={href}>
          <Heading as="h3" size="md">
            {title}
          </Heading>
        </LinkOverlay>
      </HStack>
      {children}
    </VStack>
  </LinkBox>
);
