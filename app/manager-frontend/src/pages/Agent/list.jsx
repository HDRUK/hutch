import { useDisclosure, VStack, Text } from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useAgentList } from "api/agents";
import { AgentSummary } from "components/agents/AgentSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { ActionList } from "components/ActionList";
import { DeleteModal } from "components/DeleteModal";

export const AgentsList = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedAgent, setSelectedAgent] = useState();
  const [isLoading, setIsLoading] = useState();
  const { agent } = useBackendApi();
  const { data, mutate } = useAgentList();

  const { setFilter, outputList } = useSortingAndFiltering(data, "name", {
    initialSort: { key: "name" },
    sorters: {
      name: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey: "agent",
  });

  const onDeleteAgent = async () => {
    setIsLoading(true);
    await agent.delete({ id: selectedAgent.id });
    await mutate();
    onClose();
    setIsLoading(false);
  };
  const onClickDelete = (agent) => {
    setSelectedAgent(agent);
    onOpen();
  };

  const ModalDelete = () => {
    return (
      <DeleteModal
        title={`Delete an Agent ?`}
        body={
          <VStack>
            <Text>Are you sure you want to delete this Agent:</Text>
            <Text fontWeight="bold">{selectedAgent?.name}</Text>
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
    <ActionList
      data={outputList.length > 0}
      setFilter={setFilter}
      href="/agents"
      actionTitle="Agent"
      actionNewTitle="Register an Agent"
      modalDelete={ModalDelete}
    >
      {outputList.length > 0 &&
        outputList.map((item, index) => (
          <>
            <AgentSummary
              key={index}
              href={`/agent/${item.id}`}
              onDelete={() => onClickDelete(item)}
              agentName={item.name}
              clientId={item.clientId}
              dataSources={item.dataSources}
            />
          </>
        ))}
    </ActionList>
  );
};
