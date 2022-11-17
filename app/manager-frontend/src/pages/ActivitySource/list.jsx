import { VStack, Text, useDisclosure } from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysources";
import { ActivitySourceSummary } from "components/activitysources/ActivitySourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { ActivitySourcesOrAgentsList } from "components/ActivitySourcesOrAgentsList";
import { DeleteModal } from "components/DeleteModal";

export const ActivitySourcesList = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedActivitySource, setSelectedActivitySource] = useState();
  const [isLoading, setIsLoading] = useState();
  const { activitysource } = useBackendApi();
  const { data, mutate } = useActivitySourceList();

  const { setFilter, outputList } = useSortingAndFiltering(
    data,
    "displayName",
    {
      initialSort: {
        key: "displayName",
      },
      sorters: {
        displayName: {
          sorter: (asc) => (a, b) =>
            asc ? a.localeCompare(b) : b.localeCompare(a),
        },
      },
      storageKey: "activitySource",
    }
  );

  const onDeleteSource = async () => {
    setIsLoading(true);
    await activitysource.delete({ id: selectedActivitySource.id });
    await mutate();
    onClose();
    setIsLoading(false);
  };
  const onClickDelete = (activitySource) => {
    setSelectedActivitySource(activitySource);
    onOpen();
  };

  const Delete = () => {
    return (
      <DeleteModal
        title={`Delete Activity Source?`}
        body={
          <VStack>
            <Text>Are you sure you want to delete this activity source:</Text>
            <Text fontWeight="bold">{selectedActivitySource?.displayName}</Text>
            <Text>You will not be able to reverse this!</Text>
          </VStack>
        }
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteSource}
        isLoading={isLoading}
      />
    );
  };

  return (
    <ActivitySourcesOrAgentsList
      data={data}
      setFilter={setFilter}
      href="/activitysources"
      actionName="Activity Source"
      newItemCaption="Create Activity Source"
      deleteModal={Delete}
    >
      {outputList.map((item, index) => (
        <>
          <ActivitySourceSummary
            key={index}
            href={`/activitysources/${item.id}`}
            onDelete={() => onClickDelete(item)}
            title={item.displayName}
            sourceURL={item.host}
            collectionId={item.resourceId}
          />
        </>
      ))}
    </ActivitySourcesOrAgentsList>
  );
};
