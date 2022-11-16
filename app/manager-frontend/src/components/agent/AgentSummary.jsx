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
          <Text
            color={"blue.500"}
            textTransform={"uppercase"}
            fontWeight={800}
            fontSize={"sm"}
            letterSpacing={1.1}
          >
            Agent
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

          <Heading
            color={useColorModeValue("gray.700", "white")}
            fontSize={"2xl"}
            fontFamily={"body"}
          >
            {agentName}
          </Heading>
        </Stack>
        <Stack mt={6} direction={"row"} spacing={4} align={"center"}>
          <Stack
            direction={"column"}
            spacing={0}
            fontSize={"sm"}
            align="center"
            display={"block"}
          >
            {dataSourceId && (
              <div style={{ display: "flex", alignItems: "center" }}>
                <FaDesktop />
                <Text
                  fontWeight={"700"}
                  p={1}
                  letterSpacing={0.2}
                  color={"blue.500"}
                  textTransform={"uppercase"}
                >
                  Client Id:
                </Text>
                <Text>{clientId}</Text>
              </div>
            )}

            {dataSourceId && (
              <div style={{ display: "flex", alignItems: "center" }}>
                <FaDatabase />
                <Text
                  fontWeight={"700"}
                  p={"1"}
                  letterSpacing={0.2}
                  color={"green.500"}
                  textTransform={"uppercase"}
                >
                  Datasource ID:
                </Text>
                <Text>{dataSourceId}</Text>
              </div>
            )}
          </Stack>
        </Stack>
      </Box>
    </LinkBox>
  </Center>
);
