import {
  Heading,
  VStack,
  Button,
  useDisclosure,
  Input,
  HStack,
  Text,
  InputGroup,
  InputLeftElement,
  Stack,
  Box,
} from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { AgentSummary } from "components/agent/AgentSummary";
import { useState } from "react";
import { DeleteModal } from "components/DeleteModal";
import { useNavigate } from "react-router-dom";
import { FaPlus, FaSearch, FaInfoCircle } from "react-icons/fa";
import { data } from "./AgentsList"; // Mock data for Agents list

export const AgentList = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedAgent, setSelectedAgent] = useState();
  const [isLoading, setIsLoading] = useState();
  const navigate = useNavigate();

  const {
    sorting,
    setSorting,
    onSort: handleSort,
    filter,
    setFilter,
    outputList,
  } = useSortingAndFiltering(data, "agentName", {
    initialSort: {
      key: "agentName",
    },
    sorters: {
      agentName: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey: "agent",
  });

  const onDeleteSource = async () => {
    setIsLoading(true);
    // delete agent
    // update agent list
    onClose();
    setIsLoading(false);
  };
  const onClickDelete = (agent) => {
    // assign the agent to state
    // open modal
    setSelectedAgent(agent);
    onOpen();
  };

  return (
    <>
      {data.length > 0 ? (
        <Stack w="100%" spacing={4}>
          <HStack maxW="800" w="100%" alignSelf="center" borderRadius="10px">
            <InputGroup>
              <InputLeftElement
                pointerEvents="none"
                children={<FaSearch color="gray.300" />}
              />
              <Input
                size="md"
                placeholder="Search registered Agent"
                onChange={(e) => setFilter(e.target.value)}
              />
            </InputGroup>
            <Button
              onClick={() => navigate("/agent/new")}
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

          {outputList.map((item, index) => (
            <>
              <AgentSummary
                key={index}
                href={`/agent/${item.id}`}
                onDelete={() => onClickDelete(item)}
                agentName={item.agentName}
                clientId={item.clientId}
                dataSourceId={item.dataSourceId}
              />
            </>
          ))}
        </Stack>
      ) : (
        <div>
          <Box textAlign="center" py={10} px={6}>
            <FaInfoCircle
              fontSize="2em"
              color="dodgerblue"
              style={{ display: "inline" }}
            />
            <Heading as="h2" size="xl" mb={2}>
              No Agent found.
            </Heading>
            <Button
              onClick={() => navigate("/agent/new")}
              colorScheme="green"
              leftIcon={<FaPlus />}
              width={"225"}
            >
              <Text
                textTransform="uppercase"
                fontWeight={700}
                fontSize="sm"
                letterSpacing={1.1}
              >
                Register an Agent
              </Text>
            </Button>
          </Box>
        </div>
      )}
      <DeleteModal
        title={`Delete an Agent ?`}
        body={
          <VStack>
            <Text>Are you sure you want to delete this Agent:</Text>
            <Text fontWeight="bold">{selectedAgent?.agentName}</Text>
          </VStack>
        }
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteSource}
        isLoading={isLoading}
      />
    </>
  );
};
