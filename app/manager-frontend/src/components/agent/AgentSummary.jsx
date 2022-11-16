import {
  Heading,
  HStack,
  Text,
  LinkBox,
  LinkOverlay,
  Box,
  Divider,
  Center,
  Stack,
  useColorModeValue,
  IconButton,
  VStack,
  Icon,
} from "@chakra-ui/react";
import { FaTrash, FaDesktop, FaDatabase } from "react-icons/fa";
import { Link } from "react-router-dom";

export const AgentSummary = ({
  agentName,
  href,
  clientId,
  dataSourceId,
  onDelete,
  ...p
}) => (
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
          <Box
            display="flex"
            color="blue.500"
            textTransform="uppercase"
            fontWeight={800}
            fontSize="sm"
            letterSpacing={1.1}
          >
            <Text>Agent</Text>
          </Box>

          <Divider />
          <IconButton
            h={5}
            w={5}
            icon={<FaTrash />}
            alignSelf="flex-end"
            style={{ background: "transparent" }}
            color="red.500"
            onClick={onDelete}
          />

          <HStack>
            <VStack align="left" spacing={0.5} w="full">
              <Heading
                color={useColorModeValue("gray.700", "white")}
                fontSize="2xl"
                fontFamily="body"
                mb={2.5}
              >
                {agentName}
              </Heading>

              <HStack spacing={1} alignItems="center">
                <Icon as={FaDesktop} color="gray.600" />
                <Text
                  fontWeight="700"
                  letterSpacing={0.2}
                  color="blue.500"
                  textTransform="uppercase"
                >
                  Client Id:
                </Text>
                <Text fontSize="md">{clientId}</Text>
              </HStack>
            </VStack>

            {dataSourceId && (
              <VStack
                align="end"
                fontSize="xs"
                mt={1}
                letterSpacing={0.7}
                w="30%"
                spacing={0.1}
              >
                <HStack>
                  <Icon as={FaDatabase} color="gray.500" />
                  <Text
                    color="green.500"
                    fontWeight="semibold"
                    textTransform="uppercase"
                  >
                    Datasource ID:
                  </Text>
                </HStack>
                <Text color="gray.600">{dataSourceId}</Text>
              </VStack>
            )}
          </HStack>
        </Stack>
      </Box>
    </LinkBox>
  </Center>
);
