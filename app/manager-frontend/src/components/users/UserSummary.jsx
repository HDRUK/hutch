import {
  Heading,
  Text,
  LinkBox,
  LinkOverlay,
  Box,
  Divider,
  Center,
  Stack,
  useColorModeValue,
  IconButton,
  HStack,
  VStack,
} from "@chakra-ui/react";
import { FaTrash } from "react-icons/fa";
import { Link } from "react-router-dom";

export const UserSummary = ({
  name, //user display name
  username, // username
  isUserActive, // user status
  href, // user page
  onDelete,
  ...p
}) => {
  const getNameInitials = () => {
    const splitName = name.trim().split(" "); // split name into an array
    let initials = ""; // empty initials
    splitName.forEach((name) => {
      initials = `${initials}${name.charAt(0).toUpperCase()}`; // append first char to 'initials'
    });
    return initials;
  };

  return (
    <Center py={2}>
      <LinkBox width="700px">
        <Box
          width="700px"
          bg={useColorModeValue("white", "gray.900")}
          boxShadow="lg"
          rounded="md"
          p={5}
          overflow="hidden"
          {...p}
          _hover={{
            transition: "0.3s",
            transform: "translateY(-2px)",
            boxShadow: "md",
          }}
        >
          <LinkOverlay as={Link} to={href} />
          <Stack>
            <Text
              color={"blue.500"}
              textTransform={"uppercase"}
              fontWeight={800}
              fontSize={"sm"}
              letterSpacing={1.1}
            >
              User
            </Text>
            <Divider />
            <IconButton
              h={5}
              w={5}
              icon={<FaTrash />}
              alignSelf={"flex-end"}
              style={{ background: "transparent" }}
              color={"red.500"}
              onClick={onDelete}
            />

            <HStack alignItems={"center"} spacing={3}>
              <Heading
                display="flex"
                justifyContent="center"
                alignItems="center"
                bg="gray.200"
                color="blue.900"
                borderColor="gray.300"
                borderWidth={1}
                borderRadius="50%"
                h="50px"
                w="50px"
                fontSize={"2xl"}
              >
                {isUserActive ? getNameInitials() : "?"}
              </Heading>
              <VStack alignItems="flex-start" spacing={0.2}>
                <Heading
                  color={useColorModeValue("gray.700", "white")}
                  fontSize={"2xl"}
                  fontFamily={"body"}
                >
                  {isUserActive ? name : username}
                </Heading>
                <Text color="gray.600" fontSize="sm" align="left" width="full">
                  {isUserActive ? username : "⚠️"}
                </Text>
              </VStack>
            </HStack>
          </Stack>
        </Box>
      </LinkBox>
    </Center>
  );
};
