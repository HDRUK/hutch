import {
  Heading,
  HStack,
  Icon,
  LinkBox,
  LinkOverlay,
  VStack,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";

export const ActionCard = ({ icon, title, href, children, ...p }) => (
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
