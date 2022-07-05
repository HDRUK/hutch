import {
  Heading,
  VStack,
  Link,
  Button,
  useDisclosure,
  Input,
} from "@chakra-ui/react";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { ActionCard } from "components/ActionCard";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysource";
import { ActivitySourceSummary } from "components/ActivitySourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedId, setSelectedId] = useState();
  const { activitysource } = useBackendApi();
  const { data, mutate } = useActivitySourceList();
  const {
    sorting,
    setSorting,
    onSort: handleSort,
    filter,
    setFilter,
    outputList,
  } = useSortingAndFiltering(data, "displayName", {
    initialSort: {
      key: "displayName",
    },
  });

  const onDeleteSource = async () => {
    await activitysource.delete({ id: selectedId });
    await mutate();
    onClose();
  };
  const onClickDelete = (id) => {
    setSelectedId(id);
    onOpen();
  };

  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <Heading as="h2" size="lg">
        {t("home.heading", { name: user?.fullName })}
      </Heading>
      <VStack w="100%" align="stretch">
        <Heading as="h3" size="lg">
          Activity Sources
        </Heading>
        <Link href="/activitysources/new">
          <Button>New Activity Source</Button>
        </Link>
        <Input
          placeholder="Search Activity Sources"
          onChange={(e) => setFilter(e.target.value)}
        />
        {outputList &&
          outputList.map((item, index) => (
            <ActivitySourceSummary
              key={index}
              href={`/activitysources/${item.id}`}
              onDelete={() => onClickDelete(item.id)}
              title={item.displayName}
              sourceURL={item.host}
              collectionId={item.resourceId}
            ></ActivitySourceSummary>
          ))}
      </VStack>
      <DeleteModal
        title={`Delete Activity Source ${selectedId}`}
        body="Are you sure you want to delete this activity source? You will not be able to reverse this"
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteSource}
      />
    </VStack>
  );
};
