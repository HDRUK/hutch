import { VStack, Text, useDisclosure } from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysources";
import { ActivitySourceSummary } from "components/activitysources/ActivitySourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { ActionList } from "components/ActionList";
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

  const ModalDelete = () => {
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
    <ActionList
      data={outputList.length > 0}
      setFilter={setFilter}
      href="/activitysources"
      actionTitle="Activity Source"
      actionNewTitle="Create Activity Source"
      modalDelete={ModalDelete}
    >
      {outputList.length > 0 &&
        outputList.map((item, index) => (
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
    </ActionList>
  );
};
