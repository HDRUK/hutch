import {
  Heading,
  VStack,
  Link,
  Button,
  useDisclosure,
  Input,
} from "@chakra-ui/react";

import { useDataSourceList } from "api/datasource";
import { DataSourceSummary } from "components/DataSourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";

export const DataSourcesList = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedId, setSelectedId] = useState();
  const { datasource } = useBackendApi();
  const { data, mutate } = useDataSourceList();

  const onDeleteSource = async () => {
    await datasource.delete({ id: selectedId });
    await mutate();
    onClose();
  };
  const onClickDelete = (id) => {
    setSelectedId(id);
    onOpen();
  };
  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <VStack w="100%" align="stretch">
        <Heading as="h3" size="lg">
          Data Sources
        </Heading>

        {data &&
          data.map((item, index) => (
            <DataSourceSummary
              key={index}
              href={`/datasources/${item.id}`}
              onDelete={() => onClickDelete(item.id)}
              title={""}
              lastCheckin={item.lastCheckin}
            ></DataSourceSummary>
          ))}
      </VStack>
      <DeleteModal
        title={`Delete Data Source ${selectedId}`}
        body="Are you sure you want to delete this data source? You will not be able to reverse this"
        isOpen={isOpen}
        onClose={onClose}
        id={selectedId}
        onDelete={onDeleteSource}
      />
    </VStack>
  );
};
