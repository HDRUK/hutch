import {
  Heading,
  HStack,
  Icon,
  LinkBox,
  LinkOverlay,
  VStack,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";

export const ActionCard = ({ icon, title, href, isActive, children, ...p }) => (
  <LinkBox>
    <VStack
      spacing={1}
      minW={250}
      bg={isActive && "gray.200"}
      color={isActive && "blue.900"}
      borderColor={!isActive && "gray.300"}
      borderWidth={1}
      borderRadius={5}
      h="100%"
      p={[2.5, 5]}
      align="stretch"
      _hover={{
        borderColor: `${!isActive && "blue.500"}`,
        transition: "0.45s",
      }}
      {...p}
    >
      <HStack>
        {icon && <Icon w={6} h={6} as={icon} />}
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
