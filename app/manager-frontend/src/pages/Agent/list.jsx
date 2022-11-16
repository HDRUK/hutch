import { useDisclosure, VStack, Text } from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { AgentSummary } from "components/agent/AgentSummary";
import { useState } from "react";
import { data } from "./DummyAgentsList"; // Mock data for Agents list
import { ActivitySourcesOrAgentsList } from "components/ActivitySourcesOrAgentsList";
import { DeleteModal } from "components/DeleteModal";

export const AgentsList = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedAgent, setSelectedAgent] = useState();
  const [isLoading, setIsLoading] = useState();

  const { setFilter, outputList } = useSortingAndFiltering(data, "agentName", {
    initialSort: { key: "agentName" },
    sorters: {
      agentName: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey: "agent",
  });

  const onDeleteAgent = async () => {
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

  const Delete = () => {
    return (
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
        onDelete={onDeleteAgent}
        isLoading={isLoading}
      />
    );
  };

  return (
    <ActivitySourcesOrAgentsList
      data={data}
      setFilter={setFilter}
      href="/agents"
      actionName="Agent"
      newItemCaption="Register an Agent"
      deleteModal={Delete}
    >
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
    </ActivitySourcesOrAgentsList>
  );
};
